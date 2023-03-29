namespace Gameplay.Input
{
    public class MoveInput_KeyBoard : MoveInput
    {
        public override Pega.PegaPredicted.MoveData Generate()
        {
            return new()
            {
                Vertical = UnityEngine.Input.GetAxisRaw("Vertical"),
                Horizontal = UnityEngine.Input.GetAxisRaw("Horizontal")
            };
        }
    }
}