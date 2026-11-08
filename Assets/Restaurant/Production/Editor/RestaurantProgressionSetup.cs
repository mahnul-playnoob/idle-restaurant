using System;
using UnityEditor;
using UnityEngine;
using IdleRestaurant;
public static class RestaurantProgressionSetup
{
    [MenuItem("Restaurant/Production/Apply longer progression and diverse team")]
    public static void Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play mode first");
        var catalog=AssetDatabase.LoadAssetAtPath<RestaurantCatalog>(RestaurantProductionBuilder.CatalogPath);var balance=catalog.familyBalance;
        balance.unlockBase=3000;balance.unlockGrowth=1.95;balance.unlockServicesBase=8;balance.unlockServicesPerOutlet=.75;balance.unlockUpgradeBase=12;balance.unlockUpgradesPerOutlet=2;balance.unlockUpgradeExtraCap=60;
        balance.recruitServiceBase=4;balance.recruitServiceStep=4;balance.recruitServiceCurve=.12;balance.recruitTicketStep=2;
        string[] first="Mara|Leo|Amara|Noah|Aisha|Hugo|Mina|Omar|Sofia|Linh|Kwame|Camille|Luca|Elena|Adam|Nadia|Freya|Tomas|Jisoo|Arif|Isla|Nilan|Mateo|Aminata|Julien|Soren|Priya|Harper|Ines|Layla|Nikos|Irina|Hana|Miles|Selam|Zara|Luka|Maeve|Rafi|Anya|Diego|Akua|Giulia|Ren|Samira|Erik|Beatriz|Simran|Rosa|Deniz|Bao|Fatou|Lucia|Tiago|Clara|Dawit|Mei|Yara|Emilio|Chloe|Rohan|Greta|Karim|Valeria|Andre|Minho|Dalia|Remy|Farah|Theo|Yuki".Split('|');
        string[] last="Nakamura|Okafor|Silva|Chen|Patel|Garcia|Kim|Haddad|Ivanova|Nguyen|Mensah|Dubois|Rossi|Alvarez|Kowalski|Ahmed|Larsen|Novak|Park|Rahman|Campbell|Perera|Reyes|Diop|Moreau|Andersen|Das|Bennett|Santos|Farouk|Papadopoulos|Popescu|Tanaka|Williams|Bekele|Khan|Kovac|Murphy|Chowdhury|Petrov|Lopez|Osei|Bianchi|Sato|Hassan|Johansson|Fernandes|Kaur|Torres|Demir|Tran|Kone|Martinez|Alves|Muller|Abebe|Wong|Saleh|Morales|Wilson|Gupta|Schmidt|Aziz|Diaz|Pereira|Choi|Ibrahim|Martin|Ismail|Brown|Takahashi".Split('|');
        if(first.Length!=71||last.Length!=71||catalog.managers.Length!=71)throw new Exception("Expected 71 identities");
        for(int i=0;i<71;i++)catalog.managers[i].title=first[i]+" "+last[i];
        string path="Assets/Restaurant/Production/Art/NightLamp.mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="Night lamp emission"};AssetDatabase.CreateAsset(material,path);}
        material.SetColor("_BaseColor",new Color(1,.87f,.52f));material.EnableKeyword("_EMISSION");material.SetColor("_EmissionColor",new Color(2.5f,1.6f,.5f));catalog.nightLampMaterial=material;
        EditorUtility.SetDirty(material);EditorUtility.SetDirty(balance);EditorUtility.SetDirty(catalog);EditorUtility.SetDirty(catalog.journey);AssetDatabase.SaveAssets();
        Debug.Log("LONG_PROGRESSION_CONFIGURED · 71 fictional manager identities · existing IDs preserved");
    }
}
