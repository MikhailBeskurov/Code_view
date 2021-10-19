using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class StoreManager : MonoBehaviour, IStoreListener
{
    IStoreController m_StoreContoller = null;
    private string month_1 = "subscription_1_month";
    private string month_6 = "subscription_6_months";
    private string forever = "subscription_forever";
    [SerializeField]
    private GameObject[] lock_icon = null;
    [SerializeField]
    private TextMeshProUGUI text_1_month = null;
    [SerializeField]
    private Button btn_1_month = null;
    [SerializeField]
    private TextMeshProUGUI text_6_month = null;
    [SerializeField]
    private Button btn_6_month = null;
    [SerializeField]
    private TextMeshProUGUI text_forever = null;
    [SerializeField]
    private Button btn_forever = null;
    private 
    IAppleExtensions m_AppleExtensions = null;
    bool InitializePurchasing() {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(month_1,ProductType.Subscription);
        
        builder.AddProduct(month_6, ProductType.Subscription);
        builder.AddProduct(forever, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this,builder);
      
        return true;
    }

    public void BuyProduct(string productName) {
        m_StoreContoller.InitiatePurchase(productName);
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning($"In-App Purchasing initialize failed: {error}");
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log($"In-App Purchasing soccessfully initialized");
        m_StoreContoller = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
    }
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.LogWarning($"Purchasing failed - Peoduct '{i.definition.id}', PurchaseFailureReason: {p}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        var product = e.purchasedProduct;
        if (product.definition.id == month_1) {
            Product_1month();
        }
        else if (product.definition.id == month_6)
        {
            Product_6month();
        }
        else if (product.definition.id == forever)
        {
            Product_forever();
        }

        Debug.Log($"Purchare Complite - Product: {product.definition.id} ");
        return PurchaseProcessingResult.Complete;
    }

    void Product_1month() {
        i_Subscribed_Product();
        //text_1_month.text = "Куплено";
        //btn_1_month.enabled = false;
    }
    void Product_6month()
    {
        i_Subscribed_Product();
        //text_6_month.text = "Куплено";
       // btn_6_month.enabled = false;
    }
    void Product_forever()
    {
        i_Subscribed_Product();
       // text_forever.text = "Куплено";
       // btn_forever.enabled = false;
    }

    void i_Subscribed_Product() {
        text_forever.text = "Куплено";
        btn_1_month.enabled = false;
        text_6_month.text = "Куплено";
        btn_6_month.enabled = false;
        text_1_month.text = "Куплено";
        btn_forever.enabled = false;

        for (int i = 0; i < lock_icon.Length;i++) {
            lock_icon[i].transform.parent.GetComponent<Image>().raycastTarget = true;
            lock_icon[i].SetActive(false);
        }
    }

    bool RestoreVariable() {

        //is the store initialized and available
        if (InitializePurchasing())
        {
            //get subscription info from store
            foreach (var item in m_StoreContoller.products.all)
            {
                if (item.availableToPurchase)
                {
                    // this is the usage of SubscriptionManager class
                    if (item.receipt != null) {
                        if (item.definition.type == ProductType.Subscription) {
                            Dictionary<string, string> dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
                            string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];
                                    SubscriptionManager p = new SubscriptionManager(item, intro_json);
                                    SubscriptionInfo info = p.getSubscriptionInfo();
                                    Debug.Log(info.getProductId());
                                    Debug.Log(info.getPurchaseDate());
                                    Debug.Log(info.getExpireDate());
                                    Debug.Log(info.isSubscribed());
                                    Debug.Log(info.isExpired());
                                    Debug.Log(info.isCancelled());
                                    Debug.Log(info.isFreeTrial());
                                    Debug.Log(info.isAutoRenewing());
                                    Debug.Log(info.getRemainingTime());
                                    Debug.Log(info.isIntroductoryPricePeriod());
                                    Debug.Log(info.getIntroductoryPrice());
                                    Debug.Log(info.getIntroductoryPricePeriod());
                                    Debug.Log(info.getIntroductoryPricePeriodCycles());
 
                                if (info.isSubscribed() == Result.True) {
                                    Debug.Log("The player has an active subscription, unlock the premium benefits");
                                    i_Subscribed_Product();
                                return true;
                                }
                        }

                        else {
                            Debug.LogWarning("the product is not a subscription product");
                        }

                    }

                    else {
                        Debug.LogWarning(item.definition.id + "does not have a valid receipt");
                    }

                }
            }
        }
        else
        {
            Debug.LogWarning("WARNING: Trying to check membership before store has initialized");
        }

        //if get here, the player is not an active subscriber
        return false;
    }

    void Start()
    {
        RestoreVariable();
    }
}
