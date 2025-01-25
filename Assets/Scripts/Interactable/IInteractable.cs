namespace Bubble
{
    public interface IInteractable
    {
        public void Interact(PlayerStatus playerStatus = null) {}

        public void CancelInteract() {}
    }
}
