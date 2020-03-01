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

    public string name = "";
    public int health = -1;
    public bool invulnerable = false;

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
