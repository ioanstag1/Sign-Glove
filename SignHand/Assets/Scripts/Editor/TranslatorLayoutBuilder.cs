using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TranslatorLayoutBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Build Translator Layout")]
    public static void BuildTranslatorLayout()
    {
        // 1) EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }

        // 2) Canvas
        GameObject canvasGO = new GameObject("TranslatorCanvas", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.anchorMin = Vector2.zero;
        canvasRT.anchorMax = Vector2.one;
        canvasRT.offsetMin = Vector2.zero;
        canvasRT.offsetMax = Vector2.zero;

        // ============ MAIN BACKGROUND ============
        GameObject bgGO = CreateUI("Background", canvasRT);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.93f, 0.94f, 0.96f);

        RectTransform bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // ΠΑΝΩ περιοχή (Camera + Hand) ~ 60% ύψος
        GameObject topRowGO = CreateUI("TopRow", bgRT);
        RectTransform topRT = topRowGO.GetComponent<RectTransform>();
        topRT.anchorMin = new Vector2(0, 0.40f);   // από 40% μέχρι 100%
        topRT.anchorMax = new Vector2(1, 1);
        topRT.offsetMin = Vector2.zero;
        topRT.offsetMax = Vector2.zero;

        // ΚΑΤΩ περιοχή για CHAT ~ 40% ύψος
        GameObject bottomRowGO = CreateUI("BottomRow_Chat", bgRT);
        RectTransform bottomRT = bottomRowGO.GetComponent<RectTransform>();
        bottomRT.anchorMin = new Vector2(0, 0);
        bottomRT.anchorMax = new Vector2(1, 0.40f);
        bottomRT.offsetMin = Vector2.zero;
        bottomRT.offsetMax = Vector2.zero;

        // --------------------------------------------------
        // =========== ΠΑΝΩ: CAMERA PANEL (αριστερά) =========
        // --------------------------------------------------
        GameObject camPanelGO = CreateUI("CameraPanel", topRT);
        RectTransform camRT = camPanelGO.GetComponent<RectTransform>();
        camRT.anchorMin = new Vector2(0, 0);
        camRT.anchorMax = new Vector2(0.5f, 1);          // μισό πλάτος αριστερά
        camRT.offsetMin = new Vector2(10, 10);
        camRT.offsetMax = new Vector2(-5, -10);

        var camBg = camPanelGO.AddComponent<Image>();
        camBg.color = new Color(0.12f, 0.12f, 0.14f);

        // Header "CAMERA"
        GameObject camHeader = CreateUI("CameraHeader", camRT);
        RectTransform camHeadRT = camHeader.GetComponent<RectTransform>();
        camHeadRT.anchorMin = new Vector2(0, 1);
        camHeadRT.anchorMax = new Vector2(1, 1);
        camHeadRT.pivot = new Vector2(0.5f, 1);
        camHeadRT.sizeDelta = new Vector2(0, 30);
        camHeadRT.anchoredPosition = Vector2.zero;

        Image camHeadImg = camHeader.AddComponent<Image>();
        camHeadImg.color = new Color(0.16f, 0.16f, 0.18f);

        GameObject camTxtGO = CreateUI("CameraLabel", camHeader.transform);
        TextMeshProUGUI camTxt = camTxtGO.AddComponent<TextMeshProUGUI>();
        camTxt.text = "CAMERA";
        camTxt.fontSize = 18;
        camTxt.color = Color.white;
        camTxt.alignment = TextAlignmentOptions.Center;
        Stretch(camTxtGO.GetComponent<RectTransform>());

        // Περιοχή για εικόνα κάμερας
        GameObject camViewGO = CreateUI("CameraView", camRT);
        RectTransform camViewRT = camViewGO.GetComponent<RectTransform>();
        camViewRT.anchorMin = new Vector2(0, 0);
        camViewRT.anchorMax = new Vector2(1, 1);
        camViewRT.offsetMin = new Vector2(10, 10);
        camViewRT.offsetMax = new Vector2(-10, -40);

        RawImage camRaw = camViewGO.AddComponent<RawImage>();
        camRaw.color = Color.black;
        // -> εδώ θα βάλεις WebCamTexture ή RenderTexture

        // --------------------------------------------------
        // =========== ΠΑΝΩ: HAND PANEL (δεξιά) =============
        // --------------------------------------------------
        GameObject handPanelGO = CreateUI("HandPanel", topRT);
        RectTransform handRT = handPanelGO.GetComponent<RectTransform>();
        handRT.anchorMin = new Vector2(0.5f, 0);
        handRT.anchorMax = new Vector2(1, 1);            // μισό πλάτος δεξιά
        handRT.offsetMin = new Vector2(5, 10);
        handRT.offsetMax = new Vector2(-10, -10);

        Image handBg = handPanelGO.AddComponent<Image>();
        handBg.color = new Color(0.15f, 0.16f, 0.20f);

        // Header "3D HAND"
        GameObject handHeader = CreateUI("HandHeader", handRT);
        RectTransform hhRT = handHeader.GetComponent<RectTransform>();
        hhRT.anchorMin = new Vector2(0, 1);
        hhRT.anchorMax = new Vector2(1, 1);
        hhRT.pivot = new Vector2(0.5f, 1);
        hhRT.sizeDelta = new Vector2(0, 30);
        hhRT.anchoredPosition = Vector2.zero;

        Image hhImg = handHeader.AddComponent<Image>();
        hhImg.color = new Color(0.19f, 0.19f, 0.23f);

        GameObject hhTextGO = CreateUI("HandLabel", handHeader.transform);
        TextMeshProUGUI hhText = hhTextGO.AddComponent<TextMeshProUGUI>();
        hhText.text = "3D HAND";
        hhText.fontSize = 18;
        hhText.color = Color.white;
        hhText.alignment = TextAlignmentOptions.Center;
        Stretch(hhTextGO.GetComponent<RectTransform>());

        // View για το 3D χέρι (RenderTexture από HandCamera)
        GameObject handViewGO = CreateUI("HandView", handRT);
        RectTransform handViewRT = handViewGO.GetComponent<RectTransform>();
        handViewRT.anchorMin = new Vector2(0, 0);
        handViewRT.anchorMax = new Vector2(1, 1);
        handViewRT.offsetMin = new Vector2(10, 10);
        handViewRT.offsetMax = new Vector2(-10, -40);

        RawImage handRaw = handViewGO.AddComponent<RawImage>();
        handRaw.color = Color.black;
        // -> εδώ θα βάλεις RenderTexture από μια κάμερα που βλέπει το χέρι

        // Προαιρετικό world-space anchor για το actual 3D χέρι
        GameObject handAnchor = new GameObject("HandAnchor3D");
        Undo.RegisterCreatedObjectUndo(handAnchor, "Create HandAnchor3D");
        handAnchor.transform.position = new Vector3(0, 0, 2);

        // --------------------------------------------------
        // =============== ΚΑΤΩ: CHAT PANEL =================
        // --------------------------------------------------
        GameObject chatPanelGO = CreateUI("ChatPanel", bottomRT);
        RectTransform chatRT = chatPanelGO.GetComponent<RectTransform>();
        chatRT.anchorMin = new Vector2(0, 0);
        chatRT.anchorMax = new Vector2(1, 1);
        chatRT.offsetMin = new Vector2(10, 10);
        chatRT.offsetMax = new Vector2(-10, -10);

        Image chatBg = chatPanelGO.AddComponent<Image>();
        chatBg.color = Color.white;

        // Επάνω μέρος του ChatPanel: Scroll με μηνύματα (~75% ύψος)
        GameObject scrollGO = CreateUI("ChatScrollView", chatRT);
        RectTransform scrollRT = scrollGO.GetComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0, 0.25f);
        scrollRT.anchorMax = new Vector2(1, 1);
        scrollRT.offsetMin = new Vector2(10, 10);
        scrollRT.offsetMax = new Vector2(-10, -10);

        ScrollRect sr = scrollGO.AddComponent<ScrollRect>();

        // Viewport
        GameObject vpGO = CreateUI("Viewport", scrollRT);
        RectTransform vpRT = vpGO.GetComponent<RectTransform>();
        Stretch(vpRT);
        vpGO.AddComponent<RectMask2D>();
        Image vpImg = vpGO.AddComponent<Image>();
        vpImg.color = new Color(0, 0, 0, 0);

        // Content
        GameObject contentGO = CreateUI("Content", vpRT);
        RectTransform contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 0);
        contentRT.anchorMax = new Vector2(1, 0);
        contentRT.pivot = new Vector2(0.5f, 0);
        contentRT.sizeDelta = new Vector2(0, 0);

        VerticalLayoutGroup vlg = contentGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childAlignment = TextAnchor.LowerLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = contentGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = contentRT;
        sr.viewport = vpRT;
        sr.horizontal = false;
        sr.vertical = true;

        // Κάτω μέρος του ChatPanel: input + send (~25% ύψος)
        GameObject bottomBarGO = CreateUI("BottomInputBar", chatRT);
        RectTransform bbRT = bottomBarGO.GetComponent<RectTransform>();
        bbRT.anchorMin = new Vector2(0, 0);
        bbRT.anchorMax = new Vector2(1, 0.25f);
        bbRT.offsetMin = new Vector2(10, 10);
        bbRT.offsetMax = new Vector2(-10, -10);

        Image bbImg = bottomBarGO.AddComponent<Image>();
        bbImg.color = new Color(0.97f, 0.97f, 0.97f);

        HorizontalLayoutGroup bbHLG = bottomBarGO.AddComponent<HorizontalLayoutGroup>();
        bbHLG.spacing = 8;
        bbHLG.padding = new RectOffset(8, 8, 8, 8);
        bbHLG.childAlignment = TextAnchor.MiddleCenter;
        bbHLG.childControlHeight = true;
        bbHLG.childControlWidth = true;

        // Input
        GameObject inputGO = CreateUI("ChatInputField", bottomBarGO.transform);
        LayoutElement inLE = inputGO.AddComponent<LayoutElement>();
        inLE.flexibleWidth = 1;
        inLE.minHeight = 36;

        Image inBg = inputGO.AddComponent<Image>();
        inBg.color = new Color(0.95f, 0.95f, 0.95f);
        TMP_InputField inputField = inputGO.AddComponent<TMP_InputField>();

        GameObject textAreaGO = CreateUI("TextArea", inputGO.transform);
        RectTransform taRT = textAreaGO.GetComponent<RectTransform>();
        Stretch(taRT);
        taRT.offsetMin = new Vector2(6, 4);
        taRT.offsetMax = new Vector2(-6, -4);
        inputField.textViewport = taRT;

        GameObject txtGO = CreateUI("Text", textAreaGO.transform);
        TextMeshProUGUI txt = txtGO.AddComponent<TextMeshProUGUI>();
        txt.fontSize = 16;
        txt.color = Color.black;
        txt.alignment = TextAlignmentOptions.MidlineLeft;
        Stretch(txtGO.GetComponent<RectTransform>());
        inputField.textComponent = txt;

        GameObject phGO = CreateUI("Placeholder", textAreaGO.transform);
        TextMeshProUGUI ph = phGO.AddComponent<TextMeshProUGUI>();
        ph.text = "Type message...";
        ph.fontSize = 16;
        ph.color = new Color(0, 0, 0, 0.35f);
        ph.alignment = TextAlignmentOptions.MidlineLeft;
        Stretch(phGO.GetComponent<RectTransform>());
        inputField.placeholder = ph;

        // Send button
        GameObject sendGO = CreateUI("SendButton", bottomBarGO.transform);
        LayoutElement sbLE = sendGO.AddComponent<LayoutElement>();
        sbLE.preferredWidth = 70;
        sbLE.minHeight = 36;

        Image sbImg = sendGO.AddComponent<Image>();
        sbImg.color = new Color(0.2f, 0.6f, 1f);
        Button sendBtn = sendGO.AddComponent<Button>();

        GameObject sendTxtGO = CreateUI("Label", sendGO.transform);
        TextMeshProUGUI sendTxt = sendTxtGO.AddComponent<TextMeshProUGUI>();
        sendTxt.text = "Send";
        sendTxt.fontSize = 14;
        sendTxt.fontStyle = FontStyles.Bold;
        sendTxt.color = Color.white;
        sendTxt.alignment = TextAlignmentOptions.Center;
        Stretch(sendTxtGO.GetComponent<RectTransform>());

        Selection.activeGameObject = canvasGO;
    }
#endif

    // --------- helpers ---------
    private static GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
#endif
        return go;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
