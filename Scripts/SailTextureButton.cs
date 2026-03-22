
namespace ShipyardExpansion.Scripts
{
    public class TextureButton : GoPointerButton
    {
        public override void OnActivate()
        {
            GameState.currentShipyard.sailInstaller.GetCurrentSail().GetComponent<SailTextureChanger>().NextTexture();
        }
    }
}
