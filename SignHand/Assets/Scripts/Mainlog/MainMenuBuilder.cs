using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuBuilder : MonoBehaviour
{
    // Colors
    private static Color BG_COLOR      = new Color(0.93f, 0.94f, 0.96f); 
    private static Color HEADER_COLOR  = Color.white;
    private static Color COL_GESTURE   = new Color(0.25f, 0.5f, 0.9f);  
    private static Color COL_CHAT      = new Color(0.0f, 0.65f, 0.5f);     
    private static Color COL_LEARN     = new Color(0.95f, 0.55f, 0.1f);   
    private static Color TEXT_DARK     = new Color(0.15f, 0.16f, 0.20f);
    private static Color TEXT_WHITE    = Color.white;
    private static Color TEXT_GREY     = new Color(0.4f, 0.4f, 0.45f);
    private static Color BTN_PRIMARY   = new Color(0.2f, 0.5f, 0.95f);
    private static Color BTN_SECONDARY = new Color(0.9f, 0.9f, 0.92f);
    private static Color BTN_DANGER    = new Color(0.9f, 0.3f, 0.3f);

    private const string CUSTOM_FONT_NAME = "ChatFont";

#if UNITY_EDITOR
    [MenuItem("Tools/Scenes/1. Build Main Menu UI (Requests & Friends)")]
    public static void BuildMainMenu()
    {
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(CUSTOM_FONT_NAME);

        // Cleanup
        GameObject oldCanvas = GameObject.Find("MainMenu Canvas");
        if (oldCanvas != null) DestroyImmediate(oldCanvas);
        GameObject oldES = GameObject.Find("EventSystem");
        if (oldES != null) DestroyImmediate(oldES);

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // Canvas
        GameObject rootCanvas = CreateObject("MainMenu Canvas", null);
        Canvas canvas = rootCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = rootCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        rootCanvas.AddComponent<GraphicRaycaster>();

        // Controller + Network + SceneNavigator
        GameObject controllerObj = CreateObject("AppController", rootCanvas.transform);
        MainMenuLogic logic = controllerObj.AddComponent<MainMenuLogic>();
        if (controllerObj.GetComponent<NetworkManager>() == null)
            controllerObj.AddComponent<NetworkManager>();
        SceneNavigator nav = controllerObj.AddComponent<SceneNavigator>();

        logic.uiFont   = font;
        logic.navigator = nav;

        // ---------- PANELS ----------
        // 1. START
        GameObject startPanel = CreatePanel("StartPanel", rootCanvas.transform, BG_COLOR);
        logic.startPanel = startPanel;
        GameObject startCard = CreateCardBase(startPanel.transform, 550);
        CreateText(startCard.transform, "WELCOME", 48, FontStyles.Bold, TEXT_DARK, font);
        CreateText(startCard.transform, "Sign Language Translator", 22, FontStyles.Normal, COL_GESTURE, font);
        CreateSpacer(startCard.transform, 50);
        Button goToLoginBtn = CreateButton(startCard.transform, "LOG IN", BTN_PRIMARY, TEXT_WHITE, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(goToLoginBtn.onClick, logic.ShowLogin);
        CreateSpacer(startCard.transform, 15);
        Button goToRegBtn = CreateButton(startCard.transform, "CREATE ACCOUNT", BTN_SECONDARY, TEXT_DARK, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(goToRegBtn.onClick, logic.ShowRegister);

        // 2. LOGIN
        GameObject loginPanel = CreatePanel("LoginPanel", rootCanvas.transform, BG_COLOR);
        logic.loginPanel = loginPanel;
        loginPanel.SetActive(false);
        GameObject loginCard = CreateCardBase(loginPanel.transform, 550);
        CreateText(loginCard.transform, "LOG IN", 40, FontStyles.Bold, TEXT_DARK, font);
        CreateSpacer(loginCard.transform, 30);
        logic.loginUsernameInput = CreateInputField(loginCard.transform, "Username", false, font);
        logic.loginPasswordInput = CreateInputField(loginCard.transform, "Password", true,  font);
        CreateSpacer(loginCard.transform, 10);
        GameObject logErrObj = CreateText(loginCard.transform, "", 16, FontStyles.Normal, BTN_DANGER, font);
        logic.loginErrorText = logErrObj.GetComponent<TextMeshProUGUI>();
        logErrObj.SetActive(false);
        CreateSpacer(loginCard.transform, 20);
        Button loginBtn = CreateButton(loginCard.transform, "ENTER", BTN_PRIMARY, TEXT_WHITE, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(loginBtn.onClick, logic.AttemptLogin);
        CreateSpacer(loginCard.transform, 15);
        Button backBtn1 = CreateButton(loginCard.transform, "BACK", Color.clear, TEXT_GREY, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtn1.onClick, logic.ShowStart);

        // 3. REGISTER
        GameObject regPanel = CreatePanel("RegisterPanel", rootCanvas.transform, BG_COLOR);
        logic.registerPanel = regPanel;
        regPanel.SetActive(false);
        GameObject regCard = CreateCardBase(regPanel.transform, 550);
        CreateText(regCard.transform, "NEW ACCOUNT", 40, FontStyles.Bold, TEXT_DARK, font);
        CreateSpacer(regCard.transform, 30);
        logic.regUsernameInput = CreateInputField(regCard.transform, "Username", false, font);
        logic.regEmailInput    = CreateInputField(regCard.transform, "Email",    false, font);
        logic.regPasswordInput = CreateInputField(regCard.transform, "Password", true,  font);
        CreateSpacer(regCard.transform, 10);
        GameObject regErrObj = CreateText(regCard.transform, "", 16, FontStyles.Normal, BTN_DANGER, font);
        logic.regErrorText = regErrObj.GetComponent<TextMeshProUGUI>();
        regErrObj.SetActive(false);
        CreateSpacer(regCard.transform, 20);
        Button regBtn = CreateButton(regCard.transform, "SIGN UP", BTN_PRIMARY, TEXT_WHITE, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(regBtn.onClick, logic.AttemptRegister);
        CreateSpacer(regCard.transform, 15);
        Button backBtn2 = CreateButton(regCard.transform, "BACK", Color.clear, TEXT_GREY, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtn2.onClick, logic.ShowStart);

        // 4. MAIN MENU
        GameObject menuPanel = CreatePanel("MenuPanel", rootCanvas.transform, BG_COLOR);
        logic.selectionPanel = menuPanel;
        menuPanel.SetActive(false);

        // Header
        GameObject header = CreateObject("Header", menuPanel.transform);
        Image headImg = header.AddComponent<Image>(); 
        headImg.color = HEADER_COLOR;
        header.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.1f);
        RectTransform hrt = header.GetComponent<RectTransform>();
        hrt.anchorMin = new Vector2(0, 1); 
        hrt.anchorMax = new Vector2(1, 1);
        hrt.pivot     = new Vector2(0.5f, 1);
        hrt.sizeDelta = new Vector2(0, 80);
        hrt.anchoredPosition = Vector2.zero;

        // Welcome Text
        GameObject welTxtObj = CreateText(header.transform, "Hello!", 24, FontStyles.Bold, TEXT_DARK, font);
        logic.welcomeText = welTxtObj.GetComponent<TextMeshProUGUI>();
        logic.welcomeText.alignment = TextAlignmentOptions.Left;
        RectTransform wrt = welTxtObj.GetComponent<RectTransform>();
        wrt.anchorMin = Vector2.zero; 
        wrt.anchorMax = Vector2.one; 
        wrt.offsetMin = new Vector2(50, 0); 
        wrt.offsetMax = new Vector2(-400, 0);

        // Header Actions
        GameObject headerActions = CreateObject("Actions", header.transform);
        RectTransform hart = headerActions.GetComponent<RectTransform>();
        hart.anchorMin = new Vector2(1, 0); 
        hart.anchorMax = new Vector2(1, 1);
        hart.pivot     = new Vector2(1, 0.5f);
        hart.sizeDelta = new Vector2(300, 0); 
        hart.anchoredPosition = new Vector2(-20, 0);
        HorizontalLayoutGroup halg = headerActions.AddComponent<HorizontalLayoutGroup>();
        halg.childAlignment = TextAnchor.MiddleRight; 
        halg.spacing = 15;

        Button logoutBtn = CreateButton(headerActions.transform, "LOGOUT", Color.clear, BTN_DANGER, font);
        logoutBtn.GetComponent<LayoutElement>().preferredWidth = 100;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(logoutBtn.onClick, logic.LogOut);

        Button burgerBtn = CreateButton(headerActions.transform, "FRIENDS", BTN_SECONDARY, TEXT_DARK, font);
        burgerBtn.GetComponent<LayoutElement>().preferredWidth  = 100;
        burgerBtn.GetComponent<LayoutElement>().preferredHeight = 40;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(burgerBtn.onClick, logic.ToggleFriendsMenu);

        // Main content
        GameObject mainContent = CreateObject("MainContent", menuPanel.transform);
        RectTransform mcRT = mainContent.GetComponent<RectTransform>();
        mcRT.anchorMin = Vector2.zero; 
        mcRT.anchorMax = Vector2.one; 
        mcRT.offsetMin = new Vector2(0, 0); 
        mcRT.offsetMax = new Vector2(0, -80);
        VerticalLayoutGroup mainVLG = mainContent.AddComponent<VerticalLayoutGroup>();
        mainVLG.spacing = 30; 
        mainVLG.padding = new RectOffset(0, 0, 60, 0);
        mainVLG.childAlignment = TextAnchor.UpperCenter;
        mainVLG.childControlHeight = false; 
        mainVLG.childControlWidth  = true;

        CreateText(mainContent.transform, "SELECT YOUR MODE", 42, FontStyles.Bold, TEXT_DARK, font);
        GameObject cardsRow = CreateObject("CardsRow", mainContent.transform);
        HorizontalLayoutGroup hlg = cardsRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 50; 
        hlg.childAlignment = TextAnchor.MiddleCenter; 
        hlg.childControlWidth  = false; 
        hlg.childControlHeight = false;
        cardsRow.AddComponent<LayoutElement>().minHeight = 500;

        Button btnGesture = CreateColorfulCard(cardsRow.transform, "USE GLOVE", "Sign Language", COL_GESTURE, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnGesture.onClick, logic.GoToGestureMode);

        Button btnChat = CreateColorfulCard(cardsRow.transform, "USE TEXT / VOICE", "Chat Mode", COL_CHAT, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnChat.onClick, logic.GoToChatMode);

        Button btnLearn = CreateColorfulCard(cardsRow.transform, "LEARN SIGNS", "Training", COL_LEARN, font);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnLearn.onClick, logic.GoToLearnMode);

        // Dimmer
        GameObject dimmer = CreateObject("Dimmer", menuPanel.transform);
        Image dimImg = dimmer.AddComponent<Image>();
        dimImg.color = new Color(0, 0, 0, 0.5f);
        RectTransform dimRT = dimmer.GetComponent<RectTransform>();
        dimRT.anchorMin = Vector2.zero; 
        dimRT.anchorMax = Vector2.one;
        dimRT.offsetMin = Vector2.zero; 
        dimRT.offsetMax = Vector2.zero;
        Button dimBtn = dimmer.AddComponent<Button>();
        dimBtn.transition = Selectable.Transition.None;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(dimBtn.onClick, logic.ToggleFriendsMenu);
        logic.dimmerObj = dimmer;
        dimmer.SetActive(false);

        // ---------- FRIENDS DRAWER ----------
        GameObject friendsPanel = CreatePanel("FriendsDrawer", menuPanel.transform, Color.clear);
        logic.friendsPanel = friendsPanel;

        GameObject drawer = CreateObject("DrawerBg", friendsPanel.transform);
        Image drImg = drawer.AddComponent<Image>(); 
        drImg.color = Color.white;
        drawer.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.18f);
        RectTransform drRT = drawer.GetComponent<RectTransform>();
        drRT.anchorMin = new Vector2(1, 0); 
        drRT.anchorMax = new Vector2(1, 1);
        drRT.pivot     = new Vector2(1, 0.5f);
        drRT.sizeDelta = new Vector2(380, 0);
        drRT.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup drVLG = drawer.AddComponent<VerticalLayoutGroup>();
        drVLG.padding = new RectOffset(25, 25, 24, 24);
        drVLG.spacing = 8;
        drVLG.childControlWidth      = true;
        drVLG.childForceExpandHeight = false;

        // Header
        GameObject drHeader = CreateObject("DrHeader", drawer.transform);
        HorizontalLayoutGroup dhlg = drHeader.AddComponent<HorizontalLayoutGroup>();
        dhlg.childForceExpandWidth = false; 
        dhlg.childControlWidth     = true;

        GameObject titleObj = CreateText(drHeader.transform, "FRIENDS", 26, FontStyles.Bold, TEXT_DARK, font);
        LayoutElement titleLE = titleObj.AddComponent<LayoutElement>(); 
        titleLE.flexibleWidth = 1;
        titleObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        Button closeBtn = CreateButton(drHeader.transform, "X", Color.clear, TEXT_GREY, font);
        closeBtn.GetComponent<LayoutElement>().preferredWidth  = 40;
        closeBtn.GetComponent<LayoutElement>().preferredHeight = 34;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(closeBtn.onClick, logic.ToggleFriendsMenu);

        // Search section
        CreateText(drawer.transform, "ADD NEW FRIEND", 14, FontStyles.Bold, TEXT_GREY, font)
            .GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        GameObject searchGroup = CreateObject("SearchGroup", drawer.transform);
        HorizontalLayoutGroup shlg = searchGroup.AddComponent<HorizontalLayoutGroup>();
        shlg.spacing = 10;
        shlg.childAlignment = TextAnchor.MiddleLeft;
        shlg.childControlWidth      = true;
        shlg.childForceExpandWidth  = true;
        shlg.childControlHeight     = true;
        shlg.childForceExpandHeight = false;

        LayoutElement sgLE = searchGroup.AddComponent<LayoutElement>();
        sgLE.minHeight = 45;

        TMP_InputField sInput = CreateInputField(searchGroup.transform, "Enter username...", false, font);
        logic.searchInput = sInput;

        LayoutElement inpLE = sInput.GetComponent<LayoutElement>();
        inpLE.flexibleWidth  = 1;
        inpLE.minWidth       = 220;
        inpLE.preferredWidth = 0;

        Button searchBtn = CreateButton(searchGroup.transform, "GO", BTN_PRIMARY, TEXT_WHITE, font);
        LayoutElement sbLE = searchBtn.GetComponent<LayoutElement>();
        sbLE.preferredWidth = 80;
        sbLE.minHeight      = 40;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(searchBtn.onClick, logic.SearchUser);

        // Search result text
        GameObject sResObj = CreateText(drawer.transform, "", 13, FontStyles.Bold, COL_CHAT, font);
        TextMeshProUGUI sResTMP = sResObj.GetComponent<TextMeshProUGUI>();
        sResTMP.alignment = TextAlignmentOptions.Left;
        logic.searchResultText = sResTMP;

        // SEND REQUEST button
        Button addFrBtn = CreateButton(drawer.transform, "SEND REQUEST", COL_CHAT, TEXT_WHITE, font);
        LayoutElement addFrLE = addFrBtn.GetComponent<LayoutElement>();
        addFrLE.preferredWidth = 0;
        addFrLE.minHeight      = 40;
        logic.addFriendBtn = addFrBtn.gameObject;
        addFrBtn.gameObject.SetActive(false);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(addFrBtn.onClick, logic.SendFriendRequestUI);

        // Separator line
        GameObject line = CreateObject("Line", drawer.transform);
        Image lineImg = line.AddComponent<Image>(); 
        lineImg.color = new Color(0, 0, 0, 0.06f);
        LayoutElement lineLe = line.AddComponent<LayoutElement>(); 
        lineLe.minHeight       = 1; 
        lineLe.preferredHeight = 1;

        // Pending Requests
        CreateText(drawer.transform, "PENDING REQUESTS", 13, FontStyles.Bold, BTN_DANGER, font)
            .GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        GameObject reqBox = CreateObject("RequestsBox", drawer.transform);
        Image rbImg = reqBox.AddComponent<Image>();
        rbImg.color = new Color(0.97f, 0.97f, 0.97f);
        LayoutElement rbLE = reqBox.AddComponent<LayoutElement>();
        rbLE.minHeight       = 70;
        rbLE.preferredHeight = 90;

        VerticalLayoutGroup rbVLG = reqBox.AddComponent<VerticalLayoutGroup>();
        rbVLG.padding = new RectOffset(8, 8, 6, 6);
        rbVLG.spacing = 4;
        rbVLG.childControlWidth      = true;
        rbVLG.childControlHeight     = true;
        rbVLG.childForceExpandWidth  = true;
        rbVLG.childForceExpandHeight = false;

        logic.requestsListContent = reqBox.GetComponent<RectTransform>();

        // Friends list
        CreateText(drawer.transform, "YOUR FRIENDS", 13, FontStyles.Bold, TEXT_GREY, font)
            .GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        GameObject scrollObj = CreateObject("Scroll", drawer.transform);
        Image scrImg = scrollObj.AddComponent<Image>();
        scrImg.color = new Color(0.96f, 0.96f, 0.96f);
        LayoutElement scrollLE = scrollObj.AddComponent<LayoutElement>();
        scrollLE.minHeight      = 120;
        scrollLE.flexibleHeight = 1;

        ScrollRect sr = scrollObj.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical   = true;

        GameObject viewport = CreateObject("Viewport", scrollObj.transform);
        viewport.AddComponent<Image>().color = Color.clear;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        RectTransform vpRT = viewport.GetComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero;
        vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = Vector2.zero;
        vpRT.offsetMax = Vector2.zero;
        sr.viewport = vpRT;

        GameObject content = CreateObject("Content", viewport.transform);
        RectTransform cRT = content.GetComponent<RectTransform>();
        cRT.anchorMin = new Vector2(0, 1);
        cRT.anchorMax = new Vector2(1, 1);
        cRT.pivot     = new Vector2(0.5f, 1);

        VerticalLayoutGroup cVLG = content.AddComponent<VerticalLayoutGroup>();
        cVLG.padding = new RectOffset(8, 8, 6, 6);
        cVLG.spacing = 4;
        cVLG.childControlWidth      = true;
        cVLG.childControlHeight     = true;
        cVLG.childForceExpandWidth  = true;
        cVLG.childForceExpandHeight = false;

        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sr.content = cRT;
        logic.friendsListContent = cRT;

        friendsPanel.SetActive(false);
        Selection.activeGameObject = rootCanvas;
        Canvas.ForceUpdateCanvases();
    }
#endif

    // ---------- Helpers ----------
    private static Button CreateColorfulCard(Transform parent, string title, string subtitle, Color bgCol, TMP_FontAsset font)
    {
        GameObject card = CreateObject("Card", parent);
        Image img = card.AddComponent<Image>(); 
        img.color = bgCol; 
        TryAddRoundedSprite(img);
        card.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.2f);
        card.AddComponent<Shadow>().effectDistance = new Vector2(0, -4);
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(350, 450);
        Button btn = card.AddComponent<Button>();

        VerticalLayoutGroup vlg = card.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(30, 30, 50, 50);
        vlg.spacing = 20;
        vlg.childAlignment = TextAnchor.MiddleCenter;

        CreateText(card.transform, title,    32, FontStyles.Bold,   TEXT_WHITE, font);
        CreateText(card.transform, subtitle, 18, FontStyles.Normal, new Color(1, 1, 1, 0.8f), font);
        CreateSpacer(card.transform, 30);
        GameObject enterBtnObj = CreateObject("EnterBtnVisual", card.transform);
        Image btnImg = enterBtnObj.AddComponent<Image>(); 
        btnImg.color = new Color(bgCol.r * 0.8f, bgCol.g * 0.8f, bgCol.b * 0.8f, 1f);
        TryAddRoundedSprite(btnImg);
        LayoutElement le = enterBtnObj.AddComponent<LayoutElement>();
        le.minHeight      = 50;
        le.preferredWidth = 140;
        GameObject btnTxt = CreateText(enterBtnObj.transform, "ENTER", 18, FontStyles.Bold, TEXT_WHITE, font);
        btnTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        btnTxt.GetComponent<RectTransform>().anchorMax = Vector2.one;
        return btn;
    }

    private static GameObject CreateCardBase(Transform parent, float width)
    {
        GameObject card = CreateObject("CardBase", parent);
        Image img = card.AddComponent<Image>(); 
        img.color = Color.white; 
        TryAddRoundedSprite(img);
        card.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.1f);
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, 0);
        VerticalLayoutGroup vlg = card.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(40, 40, 40, 40);
        vlg.spacing = 20;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth      = true;
        vlg.childForceExpandWidth  = true;
        vlg.childControlHeight     = true;
        card.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return card;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = CreateObject(name, parent);
        Image img = go.AddComponent<Image>(); 
        img.color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; 
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; 
        rt.offsetMax = Vector2.zero;
        return go;
    }

    private static GameObject CreateText(Transform parent, string content, int size, FontStyles style, Color color, TMP_FontAsset font)
    {
        GameObject go = CreateObject("Text", parent);
        TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
        txt.text      = content;
        txt.fontSize  = size;
        txt.fontStyle = style;
        txt.color     = color;
        if (font != null) txt.font = font;
        txt.alignment   = TextAlignmentOptions.Center;
        txt.overflowMode = TextOverflowModes.Overflow;
        return go;
    }

    private static void CreateSpacer(Transform p, float h)
    {
        GameObject go = CreateObject("Spacer", p);
        go.AddComponent<LayoutElement>().minHeight = h;
    }

    private static TMP_InputField CreateInputField(Transform parent, string placeholder, bool isPassword, TMP_FontAsset font)
    {
        GameObject go = CreateObject("Input", parent);
        Image bg = go.AddComponent<Image>(); 
        bg.color = new Color(0.96f, 0.97f, 0.98f);
        TryAddRoundedSprite(bg);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight      = 40;
        le.preferredHeight = 40;

        TMP_InputField input = go.AddComponent<TMP_InputField>();

        GameObject textArea = CreateObject("TextArea", go.transform);
        RectTransform taRT = textArea.GetComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero; 
        taRT.anchorMax = Vector2.one; 
        taRT.offsetMin = new Vector2(12, 4);
        taRT.offsetMax = new Vector2(-12, -4);
        input.textViewport = taRT;

        GameObject txtObj = CreateText(textArea.transform, "", 16, FontStyles.Normal, TEXT_DARK, font);
        TextMeshProUGUI txt = txtObj.GetComponent<TextMeshProUGUI>();
        txt.alignment = TextAlignmentOptions.MidlineLeft;
        input.textComponent = txt;

        GameObject phObj = CreateText(textArea.transform, placeholder, 16, FontStyles.Italic, new Color(0, 0, 0, 0.35f), font);
        TextMeshProUGUI ph = phObj.GetComponent<TextMeshProUGUI>();
        ph.alignment = TextAlignmentOptions.MidlineLeft;
        input.placeholder = ph;

        if (isPassword) input.contentType = TMP_InputField.ContentType.Password;

        return input;
    }

    private static Button CreateButton(Transform parent, string text, Color bgCol, Color txtCol, TMP_FontAsset font)
    {
        GameObject go = CreateObject("Btn", parent);
        Image img = go.AddComponent<Image>(); 
        img.color = bgCol; 
        TryAddRoundedSprite(img);
        Button btn = go.AddComponent<Button>();
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight      = 50;
        le.preferredWidth = 200;
        CreateText(go.transform, text, 16, FontStyles.Bold, txtCol, font);
        return btn;
    }

    private static GameObject CreateObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        if (parent != null) go.transform.SetParent(parent, false);
        return go;
    }

    private static void TryAddRoundedSprite(Image img)
    {
#if UNITY_EDITOR
        Sprite rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        if (rounded == null) rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        if (rounded != null)
        {
            img.sprite = rounded;
            img.type   = Image.Type.Sliced;
        }
#endif
    }
}
