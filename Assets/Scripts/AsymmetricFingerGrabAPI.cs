using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using UnityEngine;
using Oculus.Interaction;   

namespace ProjetReproVR
{
    /// <summary>
    /// Wraps an IFingerAPI and adds asymmetric select/unselect thresholds.
    /// selectThreshold  : score minimum pour COMMENCER à attraper (bas = facile)
    /// unselectThreshold: score minimum pour MAINTENIR le grab   (très bas = résiste aux pertes de tracking)
    /// </summary>
    public class AsymmetricFingerGrabAPI : IFingerAPI
    {
        private readonly IFingerAPI _inner;
        private readonly float _selectThreshold;
        private readonly float _unselectThreshold;

        // État interne par doigt pour l'hystérésis
        private readonly bool[] _isGrabbing = new bool[Constants.NUM_FINGERS];
        private readonly bool[] _wasGrabbing = new bool[Constants.NUM_FINGERS];

        public AsymmetricFingerGrabAPI(IFingerAPI inner,
            float selectThreshold = 0.3f,
            float unselectThreshold = 0.08f)
        {
            _inner = inner;
            _selectThreshold = selectThreshold;
            _unselectThreshold = unselectThreshold;
        }

        public void Update(IHand hand)
        {
            _inner.Update(hand);

            for (int i = 0; i < Constants.NUM_FINGERS; i++)
            {
                HandFinger finger = (HandFinger)i;
                float score = _inner.GetFingerGrabScore(finger);

                _wasGrabbing[i] = _isGrabbing[i];

                if (!_isGrabbing[i] && score >= _selectThreshold)
                    _isGrabbing[i] = true;   // attache
                else if (_isGrabbing[i] && score < _unselectThreshold)
                    _isGrabbing[i] = false;  // relâche seulement si vraiment ouvert
            }
        }

        public bool GetFingerIsGrabbing(HandFinger finger)
            => _isGrabbing[(int)finger];

        public bool GetFingerIsGrabbingChanged(HandFinger finger, bool targetState)
        {
            bool current = _isGrabbing[(int)finger];
            bool previous = _wasGrabbing[(int)finger];
            return targetState ? (!previous && current) : (previous && !current);
        }

        public float GetFingerGrabScore(HandFinger finger)
            => _inner.GetFingerGrabScore(finger);

        public Vector3 GetWristOffsetLocal()
            => _inner.GetWristOffsetLocal();
    }
}