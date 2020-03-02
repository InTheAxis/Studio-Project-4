//#define ABILITY_ADD
#define ABILITY_REPLACE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewPlayerInfo
{
    /* UI */
    public Transform self = null;
    public Transform barHolder = null;
    public List<Image> healthBarImageComps = new List<Image>();
    public TextMeshProUGUI tmName = null;

    public static List<Sprite> referenceAbilitySprites;
    public static float abilitySpacing;
    private RectTransform referenceAbilityObj;
    private List<Sprite> currentAbilities = new List<Sprite>();
    private List<RectTransform> activeAbilityObjects = new List<RectTransform>();

    public string name = "";
    public int health = -1;
    public bool invulnerable = false;
    public HumanUnlockTool attachedUnlockTool = null;

    private Color healthColor;
    private Color drainedColor;
    private Color invulColor;

    public NewPlayerInfo(Transform self)
    {
        this.self = self;
        tmName = self.Find("Name").GetComponent<TextMeshProUGUI>();
        name = tmName.text;
        barHolder = self.Find("Healthbars");
        for (int i = 0, total = barHolder.childCount; i < total; ++i)
            healthBarImageComps.Add(barHolder.GetChild(i).GetComponent<Image>());

        referenceAbilityObj = self.Find("Ability").GetComponent<RectTransform>();
        referenceAbilityObj.gameObject.SetActive(false);
    }
    public NewPlayerInfo(Transform self, string name)
    {
        this.self = self;
        tmName = self.Find("Name").GetComponent<TextMeshProUGUI>();
        this.name = name;
        tmName.text = name;
        barHolder = self.Find("Healthbars");
        for (int i = 0, total = barHolder.childCount; i < total; ++i)
            healthBarImageComps.Add(barHolder.GetChild(i).GetComponent<Image>());

        referenceAbilityObj = self.Find("Ability").GetComponent<RectTransform>();
        referenceAbilityObj.gameObject.SetActive(false);
    }
    ~NewPlayerInfo()
    {
        deregisterAbilityUnlockHandler();
    }

    public void setHealthbarColor(Color healthColor, Color drainedColor, Color invulColor)
    {
        this.healthColor = healthColor;
        this.drainedColor = drainedColor;
        this.invulColor = invulColor;

        // Update HUD elements' colors
        setHealth(health, invulnerable);
    }

    public void setHealth(int health, bool invulnerable)
    {
        this.health = health;
        this.invulnerable = invulnerable;

        if (invulnerable)
        {
            for (int i = 0; i < 3; ++i)
                setHealthbarActiveness(i, false);
            setHealthbarActiveness(3, true);

            setHealthbarObjColor(3, invulColor);
        }
        else
        {
            for(int i = 0; i < 3; ++i)
            {
                setHealthbarActiveness(i, true);

                if (i < health)
                    setHealthbarObjColor(i, healthColor);
                else
                    setHealthbarObjColor(i, drainedColor);
            }
            setHealthbarActiveness(3, false);
        }
    }
    private void setHealthbarObjColor(int index, Color color)
    {
        healthBarImageComps[index].color = color;
    }
    private void setHealthbarActiveness(int index, bool active)
    {
        healthBarImageComps[index].gameObject.SetActive(active);
    }

    public void registerAbilityUnlockHandler(HumanUnlockTool playerUnlockTool)
    {
        deregisterAbilityUnlockHandler();

        attachedUnlockTool = playerUnlockTool;
        attachedUnlockTool.abilityGainCallback += addAbility;
    }
    public void deregisterAbilityUnlockHandler()
    {
        if (attachedUnlockTool != null)
            attachedUnlockTool.abilityGainCallback -= addAbility;
    }

    public void addAbility(HumanUnlockTool.TYPE ability)
    {
        int index = (int)ability;
        if (!checkType(index))
            return;

        Sprite newAbility = referenceAbilitySprites[index];
        if (currentAbilities.Contains(newAbility))
        {
            Debug.Log("Already possessive of new ability. Do nothing.");
            return;
        }

        addAbilityObj(newAbility);
    }
    public void addAbilityObj(Sprite sprite)
    {
#if ABILITY_REPLACE

        clearAbilities();

#endif

        Debug.Log("Add ability called");
        currentAbilities.Add(sprite);
        GameObject newObj = GameObject.Instantiate(referenceAbilityObj.gameObject);
        newObj.SetActive(true);
        newObj.GetComponent<Image>().sprite = sprite;
        newObj.transform.SetParent(self);
        activeAbilityObjects.Add(newObj.GetComponent<RectTransform>());

        refreshSpriteObjs();
    }

    public void removeAbility(HumanUnlockTool.TYPE type)
    {
        int index = (int)type;
        if (!checkType(index))
            return;

        Sprite toRemoveSprite = referenceAbilitySprites[index];
        if (currentAbilities.Contains(toRemoveSprite))
        {
            Debug.Log("Removing sprite");
            int toRemoveIndex = currentAbilities.FindIndex(obj => obj == toRemoveSprite);

            currentAbilities.RemoveAt(toRemoveIndex);
            GameObject.Destroy(activeAbilityObjects[toRemoveIndex].gameObject);
            activeAbilityObjects.RemoveAt(toRemoveIndex);

            refreshSpriteObjs();
        }
        else
            Debug.Log("Could not find sprite to remove");
    }
    public void clearAbilities()
    {
        foreach (RectTransform obj in activeAbilityObjects)
            GameObject.Destroy(obj.gameObject);
        currentAbilities.Clear();
        activeAbilityObjects.Clear();
    }

    private void refreshSpriteObjs()
    {
        if (activeAbilityObjects.Count < 1)
            return;

        RectTransform prevObj = activeAbilityObjects[0];
        prevObj.localPosition = referenceAbilityObj.localPosition;
        prevObj.localRotation = referenceAbilityObj.localRotation;
        prevObj.localScale = referenceAbilityObj.localScale;

        for (int i = 1, total = activeAbilityObjects.Count; i < total; ++i)
        {
            RectTransform thisObj = activeAbilityObjects[i];
            thisObj.localRotation = prevObj.localRotation;
            thisObj.localScale = referenceAbilityObj.localScale;

            Vector3 localRight = -prevObj.right;
            thisObj.localPosition = prevObj.localPosition + -localRight * (prevObj.rect.width * 0.5f + thisObj.rect.width * 0.5f + abilitySpacing);

            prevObj = thisObj;
        }
    }

    private bool checkType(HumanUnlockTool.TYPE type)
    {
        return checkType((int)type);
    }
    private bool checkType(int index)
    {
        if (index < 0)
            return false;
        else if (index >= referenceAbilitySprites.Count)
        {
            Debug.LogError("Ability sprite not found!");
            return false;
        }
        return true;
    }

    public void setName(string name)
    {
        this.name = name;
        tmName.text = name;
    }

    public void setAll(string name, int health, bool invulnerable)
    {
        setName(name);
        setHealth(health, invulnerable);
    }

    public void setActive(bool isActive)
    {
        self.gameObject.SetActive(isActive);
    }
}
