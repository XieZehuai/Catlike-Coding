using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Testing
{
	[RequireComponent(typeof(Animator))]
	public class PlayableAnimation : MonoBehaviour
	{
		public AnimationClip clip;

        private Animator animator;
		private PlayableGraph playableGraph;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            playableGraph = PlayableGraph.Create("PlayableAnimation");
            var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
            var playableClip = AnimationClipPlayable.Create(playableGraph, clip);
            playableOutput.SetSourcePlayable(playableClip);
            playableGraph.Play();
        }
    }
}
