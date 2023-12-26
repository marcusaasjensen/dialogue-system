using System.Collections.Generic;
using DialogueSystem.Data;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Runtime.UI
{
    [ExecuteAlways]
    public class TextAnimator : MonoBehaviour
    {
        [Header("To Animate"), SerializeField] private TMP_Text textMesh;
        #if UNITY_EDITOR
        [Header("Editor Mode"), SerializeField] private bool testAnimationInEditor;
        #endif
        [SerializeField] private TextAnimationType testAnimationType;
        [SerializeField, Min(0)] private int testStartPosition;
        [SerializeField, Min(0)] private int testEndPosition;
        [Header("Default Values"), SerializeField] private float animationSpeed = 1.0f;
        [SerializeField] private float amount = 1.0f;
        [SerializeField] private bool synchronizeTextAnimation;
        
        private readonly List<TextAnimation> _textAnimations = new ();
        private bool _isTextMeshNull;

        private void Awake()
        {
            _isTextMeshNull = textMesh == null;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                testAnimationInEditor = false;
                return;
            }
            _isTextMeshNull = textMesh == null;

            if (!testAnimationInEditor)
            {
                EditorApplication.update -= AnimateTextMesh;
                ResetAnimator();
                return;
            }
            ResetAnimator();
            PlayAnimationInEditor();
            EditorApplication.update += AnimateTextMesh;
        }
        #endif
        
        private void PlayAnimationInEditor() => PlayAnimation(testAnimationType, testStartPosition, testEndPosition, null, null, null);

        private void Update() => AnimateTextMesh();

        private void AnimateTextMesh()
        {
            if (_isTextMeshNull)
            {
                return;
            }

            textMesh.ForceMeshUpdate();
            var mesh = textMesh.mesh;
            var vertices = mesh.vertices;
            
            foreach (var textAnimation in _textAnimations)
            {
                var textEndPosition = textAnimation.EndPosition < 0 ? textMesh.textInfo.characterCount : textAnimation.EndPosition;
                
                for (var i = textAnimation.StartPosition; i < textEndPosition && i < textMesh.textInfo.characterCount; i++)
                {
                    var c = textMesh.textInfo.characterInfo[i];
                    if (c.character == ' ' || c.vertexIndex < 0 || c.vertexIndex + 3 >= vertices.Length)
                    {
                        continue;
                    }
                    
                    var vertexIndex = c.vertexIndex;

                    var sync = textAnimation.Sync ?? synchronizeTextAnimation;
                    
                    Vector3 offset = textAnimation.GetOffset(sync ? Time.time : Time.time + i, animationSpeed, amount);
                    vertices[vertexIndex] += offset;
                    vertices[vertexIndex + 1] += offset;
                    vertices[vertexIndex + 2] += offset;
                    vertices[vertexIndex + 3] += offset;
                }
            }
            
            mesh.vertices = vertices;
            textMesh.canvasRenderer.SetMesh(mesh);
        }

        public void PlayAnimation(TextAnimationType animationType, int startPosition, int endPosition, float? speedValue, float? amountValue, bool? sync) => _textAnimations.Add(new TextAnimation(animationType, startPosition, endPosition, speedValue, amountValue, sync));

        public void ResetAnimator()
        {
            _textAnimations.Clear();
        }

        private class TextAnimation
        {
            private TextAnimationType AnimationType { get; }
            public int StartPosition { get; }
            public int EndPosition { get; }
            public bool? Sync { get; }
            private float? Speed { get; }
            private float? Amount { get; }

            public TextAnimation(TextAnimationType animationType, int startPosition, int endPosition, float? speed, float? amount, bool? sync)
            {
                AnimationType = animationType;
                StartPosition = startPosition;
                EndPosition = endPosition;
                Speed = speed;
                Amount = amount;
                Sync = sync;
            }

            public Vector2 GetOffset(float time, float defaultSpeed, float defaultAmount)
            {
                var adjustedTime = Speed == null ? time * defaultSpeed : time * Speed.Value;
                var newAmount = Amount ?? defaultAmount;
                
                switch (AnimationType)
                {
                    case TextAnimationType.Wobble:
                        return new Vector2(Mathf.Sin(adjustedTime * 3.3f), Mathf.Cos(adjustedTime * 2.5f)) * newAmount;
                    case TextAnimationType.Shake:
                        return new Vector2(Mathf.PerlinNoise(adjustedTime * 5.0f, 0.0f), 0.0f) * newAmount;
                    case TextAnimationType.Wave:
                        return new Vector2(0.0f, Mathf.Sin(adjustedTime * 2.0f)) * newAmount;
                    case TextAnimationType.PingPong:
                        var alpha = Mathf.PingPong(adjustedTime, 1.0f);
                        return new Vector2(0.0f, alpha - 0.5f) * newAmount;
                    case TextAnimationType.Flicker:
                        var flickerAmount = Mathf.PerlinNoise(adjustedTime * 10.0f, 0.0f) * 0.5f;
                        return new Vector2(flickerAmount, flickerAmount) * newAmount;
                    case TextAnimationType.Spiral:
                        var spiralRadius = Mathf.Sin(adjustedTime) * 0.5f + 1.0f;
                        var spiralAngle = adjustedTime * 180.0f;
                        return new Vector2(Mathf.Cos(spiralAngle), Mathf.Sin(spiralAngle)) * (spiralRadius * newAmount);
                    case TextAnimationType.Jitter:
                        return new Vector2(Random.Range(-newAmount, newAmount), Random.Range(-newAmount, newAmount));
                    case TextAnimationType.None:
                    default:
                        return Vector2.zero;
                }
            }
        }

    }
}
