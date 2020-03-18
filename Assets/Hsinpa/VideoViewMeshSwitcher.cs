using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using RenderHeads.Media.AVProVideo;


public class VideoViewMeshSwitcher : MonoBehaviour
{
    [SerializeField]
    MeshFilter mesh;

    [SerializeField]
    Renderer renderer;

    [SerializeField]
    private MediaPlayer avplayer;

    [SerializeField]
    private ApplyToMesh applyToMesh;

    [SerializeField]
    private Mesh[] meshTypes;

    [SerializeField]
    private Material[] materialTypes;

    private int currentIndex = 0;

    private enum SteroType { None, Top_Bottom, Left_Right, Custom_UV };
    private SteroType steroType;

    private void Start()
    {
        steroType = SteroType.None;
        SubscribeKeyboardEvent();
        RotateViewType(currentIndex);
    }

    private void SubscribeKeyboardEvent() {

        var keyboardObservable = Observable.EveryUpdate().Where(_ => Input.anyKeyDown);

        keyboardObservable.Buffer(keyboardObservable.Throttle(System.TimeSpan.FromMilliseconds(250)))
          .Where(xs => xs.Count >=1)
          .Subscribe(xs =>
          {
              currentIndex = (currentIndex + 1) % meshTypes.Length;
              RotateViewType(currentIndex);
              Debug.Log("DoubleClick Detected! Count:" + xs.Count);
          });
    }

    private void RotateViewType(int index) {
        mesh.mesh = meshTypes[index];
        renderer.material = materialTypes[index];

        if (IsSteroVideoType(renderer.material.name)) {
            renderer.material.SetFloat("Stereo", (int)SteroType.Left_Right);
        }
        
        applyToMesh.enabled = false;

        _ = Utility.UtilityMethod.DoDelayWork(0.1f, () => {
            applyToMesh.enabled = true;
        });
    }

    private bool IsSteroVideoType(string name) {
        return (renderer.material.name.IndexOf("360") >= 0 || renderer.material.name.IndexOf("180") >= 0);
    }

}
