using TMPro;  // Namespace for TextMeshPro
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public int wood = 50;
    public int metal = 75;  // starting metal for towers
    public int lives = 20;

    [Header("UI References")]

    public TMP_Text woodText;
    public TMP_Text metalText;
    public TMP_Text livesText;

    private void Start()
    {
        UpdateUI();
    }


    private bool SpendResource(ref int resource, int amount)
    {
        if (resource >= amount)
        {
            resource -= amount;
            return true;
        }
        return false;
    }


    private void UpdateUI()
    {

        woodText.text = wood.ToString();
        metalText.text = metal.ToString();
        livesText.text = lives.ToString();
    }



    public bool SpendWood(int amount)
    {
        if (SpendResource(ref wood, amount))
        {
            woodText.text = wood.ToString();
            return true;
        }
        return false;
    }

    public bool SpendMetal(int amount)
    {
        if (SpendResource(ref metal, amount))
        {
            metalText.text = metal.ToString();
            return true;
        }
        return false;
    }



    public void GainWood(int amount)
    {
        wood += amount;
        woodText.text = wood.ToString();
    }

    public void GainMetal(int amount)
    {
        metal += amount;
        metalText.text = metal.ToString();
    }

    public void LoseLife(int amount = 1)
    {
        lives -= amount;
        livesText.text = lives.ToString();
        if (lives <= 0)
        {
            // Game Over Logic
        }
    }
}