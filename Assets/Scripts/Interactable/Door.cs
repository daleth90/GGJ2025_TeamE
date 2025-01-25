namespace Bubble
{
    public class Door : KeyInteractable
    {
        public override void InteractWithKey()
        {
            // Play Animation
            // Play Sound Effect
            gameObject.SetActive(false);
        }
    }
}