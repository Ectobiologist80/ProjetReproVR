using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using System.Reflection;
using UnityEngine;
using Oculus.Interaction;

namespace ProjetReproVR
{
    public class HandGrabAPIThresholdInjector : MonoBehaviour
    {
        [Header("Seuil d'attache")]
        [Range(0f, 1f)]
        [SerializeField] private float _selectThreshold = 0.30f;

        [Header("Seuil de relâchement")]
        [Range(0f, 1f)]
        [SerializeField] private float _unselectThreshold = 0.08f;

        private void Start()
        {
            var api = GetComponent<HandGrabAPI>();
            if (api == null)
            {
                Debug.LogError("[ThresholdInjector] HandGrabAPI introuvable.");
                return;
            }

            // Récupère les instances déjà créées par HandGrabAPI.Start()
            // au lieu d'en créer de nouvelles
            var type = typeof(HandGrabAPI);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var pinchField = type.GetField("_fingerPinchGrabAPI", flags);
            var palmField  = type.GetField("_fingerPalmGrabAPI",  flags);

            var existingPinch = pinchField?.GetValue(api) as IFingerAPI;
            var existingPalm  = palmField?.GetValue(api)  as IFingerAPI;

            if (existingPinch == null || existingPalm == null)
            {
                Debug.LogError("[ThresholdInjector] Impossible de récupérer les FingerAPI. " +
                    "Vérifie que HandGrabAPI.Start() s'est bien exécuté avant.");
                return;
            }

            // Wrap les instances existantes (bon type, bons joints)
            api.InjectOptionalFingerPinchAPI(
                new AsymmetricFingerGrabAPI(existingPinch, _selectThreshold, _unselectThreshold));
            api.InjectOptionalFingerGrabAPI(
                new AsymmetricFingerGrabAPI(existingPalm,  _selectThreshold, _unselectThreshold));

            Debug.Log("[ThresholdInjector] Seuils injectés avec succès.");
        }
    }
}