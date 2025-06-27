using UnityEngine;
using UnityEngine.UIElements;

public class TestNewFeaturesUI : MonoBehaviour
{
    [SerializeField] UIDocument m_uiDocument;
    ScrollView m_scrollView;

    readonly TestRefTypes m_testRefTypes = new();
    readonly SpanExample m_spanExample = new();
    void Awake() {
        if (m_uiDocument == null) {
            Debug.LogError("UIDocument is not assigned.");
            return;
        }

        m_scrollView = m_uiDocument.rootVisualElement.Q<ScrollView>();
        m_testRefTypes.onInfo += AddLabelWithContent;
        m_spanExample.onInfo += AddLabelWithContent;
    }

    void Start() {
        if (m_scrollView == null) {
            Debug.LogError("ScrollView is not initialized.");
            return;
        }

        // Clear previous content
        m_scrollView.Clear();

        // Start tests
        m_testRefTypes.StartTest();
        m_spanExample.StartTest();
    }

    void OnDestroy() {
        if (m_testRefTypes != null) {
            m_testRefTypes.onInfo -= AddLabelWithContent;
        }
        if (m_spanExample != null) {
            m_spanExample.onInfo -= AddLabelWithContent;
        }
    }
    
    
    void AddLabelWithContent(string content)
    {
        if (m_scrollView == null) {
            Debug.LogError("ScrollView is not initialized.");
            return;
        }

        Label label = new Label(content);
        m_scrollView.Add(label);
    }
}
