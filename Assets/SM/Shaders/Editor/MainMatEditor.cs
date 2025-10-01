public class MainMatEditor : CustomMaterialEditor
{
	protected override void CreateToggleList()
	{   
		Toggles.Add(new FeatureToggle("Use Textures","MainTex","USETEX_ON","USETEX_OFF"));
	}
}