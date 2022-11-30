using UnityEngine;

namespace Assets.Scripts.UI {
    class APIGatewayHUD : MonoBehaviour {
        void OnGUI() {
            GUILayout.BeginArea(new Rect(Screen.width - 500, 50, 490, 100));
            if (!APIGateway.instance.IsAuthSessionActive) {
                GUILayout.BeginHorizontal();
                APIGateway.instance.SetBaseURL(GUILayout.TextField(APIGateway.instance.BaseURL));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }
}
