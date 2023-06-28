using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.Core.Assets;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions;

public class BuildableAbstractionsPlugin : MonoBehaviour, IRocketPlugin
{
    public string Name { get; }
    public PluginState State { get; }
    public TranslationList DefaultTranslations { get; }
    public IAsset<TranslationList> Translations { get; }

    public BuildableAbstractionsPlugin()
    {
        State = PluginState.Unloaded;
        Name = "BuildableAbstractions";
        DefaultTranslations = new TranslationList();
        Translations = new Asset<TranslationList>();
        State = PluginState.Loaded;
    }

    public T TryAddComponent<T>() where T : Component
    {
        return gameObject.TryAddComponent<T>();
    }

    public void TryRemoveComponent<T>() where T : Component
    {
        gameObject.TryRemoveComponent<T>();
    }
}