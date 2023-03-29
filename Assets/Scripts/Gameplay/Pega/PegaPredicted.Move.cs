using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Gameplay.Input;
using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Pega
{
    public partial class PegaPredicted
    {
        #region Types

        public struct MoveData : IReplicateData
        {
            public float Horizontal;
            public float Vertical;

            public MoveData(float horizontal, float vertical)
            {
                Horizontal = horizontal;
                Vertical = vertical;
                _tick = 0;
            }


            public bool IsNull => Horizontal == 0 && Vertical == 0;


            private uint _tick;

            public void Dispose()
            {
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        public struct ReconcileData : IReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;

            public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                _tick = 0;
            }

            private uint _tick;

            public void Dispose()
            {
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        #endregion


        [SyncVar] public bool canMove;
        private MoveInput _moveInput;


        private void Move_OnTick()
        {
            //AddGravity
            _rigidbody.AddForce(_cTransform.up * (Physics.gravity.y * 2), ForceMode.Acceleration);

            if (IsOwner)
            {
                Reconciliation(default, false);
                BuildMoveData(out var md);
                Move(md, false);
            }

            if (IsServer)
            {
                var isBot = OwnerId < 0;

                if (isBot)
                {
                    //AI
                }
                else
                {
                    Move(default, true);
                }
            }
        }

        private void Move_OnPostTick()
        {
            if (!IsServer) return;

            var rd = new ReconcileData(_cTransform.position, _cTransform.rotation, _rigidbody.velocity);
            Reconciliation(rd, true);
        }

        private void BuildMoveData(out MoveData md)
        {
            md = default;

            if (canMove)
            {
                var moveData = _moveInput.Generate();

                if (moveData.IsNull) return;
                md = moveData;
            }
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
        {
            MoveProcess(md);

            if (IsServer)
            {
                GameplayController.Singleton.SetVelocity(OwnerId, _rigidbody.velocity.magnitude);
            }
        }

        private void MoveProcess(MoveData md)
        {
            if (!canMove || md.IsNull) return;

            if (GameplayController.Singleton.state != GameState.Playing)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            var deltaTime = (float)TimeManager.TickDelta;
            var lastVelocity = _rigidbody.velocity;

            var currentSpeed = lastVelocity.magnitude;
            var accelRampT = currentSpeed / ValueOf(PegaStat.MaxSpeed);
            const float multipliedAccelerationCurve = 5f;
            var accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, Mathf.Pow(accelRampT, 2));

            var acceleration = ValueOf(PegaStat.Acceleration) * accelRamp;
            var turningPower = md.Horizontal * ValueOf(PegaStat.TurnRate);

            // calculate movement
            var turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
            var fwd = turnAngle * transform.forward;
            var movement = fwd * (md.Vertical * acceleration);

            // if over max speed, cannot accelerate faster.
            var overMaxSpeed = currentSpeed >= ValueOf(PegaStat.MaxSpeed);
            if (overMaxSpeed) movement *= 0.0f;

            // calculate new velocity
            var newVelocity = lastVelocity + movement * deltaTime;
            newVelocity.y = lastVelocity.y;

            //  clamp max speed
            if (!overMaxSpeed) newVelocity = Vector3.ClampMagnitude(newVelocity, ValueOf(PegaStat.MaxSpeed));

            // APPLY NEW VELOCITY
            _rigidbody.velocity = newVelocity;

            // ROTATE
            transform.Rotate(Vector3.up, turningPower * deltaTime, Space.Self);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer, Channel channel = Channel.Unreliable)
        {
            _cTransform.position = rd.Position;
            _cTransform.rotation = rd.Rotation;

            _rigidbody.velocity = rd.Velocity;
        }
    }
}