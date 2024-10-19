#pragma warning disable 649

using UnityEngine;
using UnityEngine.Serialization;

namespace CC.DialogueSystem
{
    public class CharacterSpriteLoader : MonoBehaviour
    {
        [SerializeField] private DialogueSprites_SO _spritesObject;
        [FormerlySerializedAs("_registerOnStart")] [SerializeField] private bool registerOnAwake;

        #region MonoBehaviour

        // Initialize
        private void Awake()
        {
            if (registerOnAwake)
                LoadSprites();
        }

        #endregion

        // Send the character sprites to the repo
        public void LoadSprites()
        {
            // Can't load if there's no name
            if (string.IsNullOrEmpty(_spritesObject.CharactersName))
            {
                DialogueLogger.LogError($"Object {gameObject.name} cannot register character sprites without a name");
                return;
            }

            // Can't load if there's no sprites object
            if (_spritesObject == null)
            {
                DialogueLogger.LogError($"Object {gameObject.name} has an empty sprites object variable");
                return;
            }

            SpriteRepo.Instance.RegisterCharacterSprites(_spritesObject);
        }
    }
}