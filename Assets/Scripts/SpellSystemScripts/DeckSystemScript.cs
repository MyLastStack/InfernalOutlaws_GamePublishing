using MagicPigGames;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DeckSystemScript : MonoBehaviour
{
    [Header("Keybind Inputs")]
    public InputAction input1;
    public InputAction input2;
    public InputAction input3;

    [Header("Spell Deck")]
    public List<Spell> totalSpells = new List<Spell>();
    public List<Spell> shuffledSpells = new List<Spell>();

    [Header("Object References")]
    [SerializeField] public Camera cam;
    [SerializeField] public GameObject spellSpawnArea;

    [Header("Spell Prefab")]
    [SerializeField] public GameObject fireBallPrefab;
    [SerializeField] public GameObject lightningStrikePrefab;
    [SerializeField] public GameObject toxicCloudPrefab;
    [SerializeField] public GameObject warpedWorldPrefab;

    [Header("Spell Sprites")]
    [SerializeField] public Sprite fireBallSprite;
    [SerializeField] public Sprite lightningStrikeSprite;
    [SerializeField] public Sprite toxicCloudSprite;
    [SerializeField] public Sprite revitilizeSprite;
    [SerializeField] public Sprite warpedWorldSprite;

    float currentCDTimer = 2;
    float spellCooldownDuration = 2;
    private bool isCooldown = false;

    [Header("Object References")]
    public GameObject cardSlotOne;
    public GameObject cardSlotTwo;
    public GameObject cardSlotThree;
    public Image csOne;
    public Image csTwo;
    public Image csThree;
    public TMP_Text csText;
    public TMP_Text csText2;
    public TMP_Text csText3;
    public VerticalProgressBar bar;

    FireballSpell fireballSpell;
    LightningStrikeSpell lightningStrikeSpell;
    ToxicCloudSpell toxicCloudSpell;
    RevitilizeSpell revitilizeSpell;
    WarpedWorldSpell warpedWorldSpell;

    #region Base Spell Class
    [Serializable]
    public class Spell
    {
        // Spell Template
        protected Sprite spellSprite;
        protected string spellName;
        protected Color spellNameColor;

        public Spell(Sprite sprite, string name, Color color)
        {
            spellSprite = sprite;
            spellName = name;
            spellNameColor = color;
        }

        public virtual void Cast(GameObject spawnPoint) { }
        public virtual Sprite SpellSprite
        {
            get { return spellSprite; }
        }
        public virtual string SpellName
        {
            get { return spellName; }
        }
        public virtual Color SpellNameColor 
        { 
            get { return spellNameColor; } 
        }
    }
    #endregion

    #region Unique Spell Classes
    public class FireballSpell : Spell
    {
        public GameObject fireballPrefab;

        public FireballSpell(Sprite sprite, string name, Color color) : base(sprite, name, color)
        {
        }

        public override void Cast(GameObject spawnPoint)
        {
            var pos = spawnPoint.transform.position + spawnPoint.transform.forward;
            var rot = spawnPoint.transform.rotation;
            Instantiate(fireballPrefab, pos, rot);
        }
    }

    public class LightningStrikeSpell : Spell
    {
        public Camera lsCam;
        public GameObject lightningStrikePrefab;

        public LightningStrikeSpell(Sprite sprite, string name, Color color) : base(sprite, name, color)
        {
        }

        public override void Cast(GameObject spawnPoint)
        {
            RaycastHit hit;
            if (Physics.Raycast(lsCam.transform.position, lsCam.transform.forward, out hit, 100f))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Instantiate(lightningStrikePrefab, hit.point, rotation);
            }
        }
    }

    public class ToxicCloudSpell : Spell
    {
        public Camera tcCam;
        public GameObject toxicCloudPrefab;

        public ToxicCloudSpell(Sprite sprite, string name, Color color) : base(sprite, name, color)
        {
        }

        public override void Cast(GameObject spawnPoint)
        {
            RaycastHit hit;
            if (Physics.Raycast(tcCam.transform.position, tcCam.transform.forward, out hit, 100f))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Instantiate(toxicCloudPrefab, hit.point, rotation);
            }
        }
    }

    public class RevitilizeSpell : Spell
    {
        PlayerController playerHP;

        public RevitilizeSpell(Sprite sprite, string name, Color color) : base(sprite, name, color)
        {
        }

        public override void Cast(GameObject spawnPoint)
        {
            playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            if ((playerHP.ps.maxHealth - playerHP.ps.health) < 10)
            {
                playerHP.ps.health += (playerHP.ps.maxHealth - playerHP.ps.health);
            }
            else
            {
                playerHP.ps.health += 10f;
            }
        }
    }

    public class WarpedWorldSpell: Spell
    {
        public Camera wwCam;
        public GameObject warpedWorldPrefab;

        public WarpedWorldSpell(Sprite sprite, string name, Color color) : base(sprite, name, color)
        {
        }

        public override void Cast(GameObject spawnPoint)
        {
            RaycastHit hit;
            if (Physics.Raycast(wwCam.transform.position, wwCam.transform.forward, out hit, 100f))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Instantiate(warpedWorldPrefab, hit.point, rotation);
            }
        }
    }
    #endregion

    void Start()
    {
        SpellSetup();

        totalSpells.Add(fireballSpell);
        totalSpells.Add(fireballSpell);
        totalSpells.Add(lightningStrikeSpell);
        totalSpells.Add(lightningStrikeSpell);
        totalSpells.Add(toxicCloudSpell);
        totalSpells.Add(toxicCloudSpell);
        totalSpells.Add(revitilizeSpell);
        totalSpells.Add(warpedWorldSpell);
    }

    void Update()
    {
        if (totalSpells.Count != 0)
        {
            if (shuffledSpells.Count == 0)
            {
                ShuffleDeck(totalSpells);
            }
            else
            {
                SpellCasting();
            }
        }

        if (currentCDTimer < spellCooldownDuration)
        {
            currentCDTimer += Time.deltaTime;
        }
        if (currentCDTimer > spellCooldownDuration)
        {
            currentCDTimer = spellCooldownDuration;
        }
        bar.SetProgress(currentCDTimer / spellCooldownDuration);

        // Visuals
        for (int i = 0; i < 3; i++)
        {
            if (i < shuffledSpells.Count)
            {
                switch (i)
                {
                    case 0:
                        cardSlotOne.SetActive(true);
                        csOne.sprite = shuffledSpells[i].SpellSprite;
                        csText.text = shuffledSpells[i].SpellName;
                        csText.color = shuffledSpells[i].SpellNameColor;
                        break;
                    case 1:
                        cardSlotTwo.SetActive(true);
                        csTwo.sprite = shuffledSpells[i].SpellSprite;
                        csText2.text = shuffledSpells[i].SpellName;
                        csText2.color = shuffledSpells[i].SpellNameColor;
                        break;
                    case 2:
                        cardSlotThree.SetActive(true);
                        csThree.sprite = shuffledSpells[i].SpellSprite;
                        csText3.text = shuffledSpells[i].SpellName;
                        csText3.color = shuffledSpells[i].SpellNameColor;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        cardSlotOne.SetActive(false);
                        break;
                    case 1:
                        cardSlotTwo.SetActive(false);
                        break;
                    case 2:
                        cardSlotThree.SetActive(false);
                        break;
                }
            }
        }
    }

    private void SpellSetup()
    {
        fireballSpell = new FireballSpell(fireBallSprite, "Fireball", new Color(1f, 0.196f, 0f, 1f));
        fireballSpell.fireballPrefab = fireBallPrefab;

        lightningStrikeSpell = new LightningStrikeSpell(lightningStrikeSprite, "Lightning", new Color(0f, 0.973f, 1f, 1f));
        lightningStrikeSpell.lsCam = cam;
        lightningStrikeSpell.lightningStrikePrefab = lightningStrikePrefab;

        toxicCloudSpell = new ToxicCloudSpell(toxicCloudSprite, "Toxic Cloud", Color.green);
        toxicCloudSpell.tcCam = cam;
        toxicCloudSpell.toxicCloudPrefab = toxicCloudPrefab;

        revitilizeSpell = new RevitilizeSpell(revitilizeSprite, "Revitilize", new Color(1f, 0.796f, 0f, 1f));

        warpedWorldSpell = new WarpedWorldSpell(warpedWorldSprite, "Warped World", Color.white);
        warpedWorldSpell.wwCam = cam;
        warpedWorldSpell.warpedWorldPrefab = warpedWorldPrefab;
    }

    public void ShuffleDeck(List<Spell> shuffling)
    {
        shuffledSpells.Clear();
        shuffledSpells.AddRange(totalSpells);

        System.Random rng = new System.Random();
        int n = shuffledSpells.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Spell temp = shuffledSpells[k];
            shuffledSpells[k] = shuffledSpells[n];
            shuffledSpells[n] = temp;
        }
    }

    public void SpellCasting()
    {
        if (!isCooldown)
        {
            if (input1.WasPressedThisFrame())
            {
                shuffledSpells[0].Cast(spellSpawnArea);
                shuffledSpells.RemoveAt(0);
                StartCoroutine(SpellCooldown());
            }
            if (input2.WasPressedThisFrame())
            {
                if (shuffledSpells.Count > 1)
                {
                    shuffledSpells[1].Cast(spellSpawnArea);
                    shuffledSpells.RemoveAt(1);
                    StartCoroutine(SpellCooldown());
                }
                else
                {
                    return;
                }
            }
            if (input3.WasPressedThisFrame())
            {
                if (shuffledSpells.Count > 2)
                {
                    shuffledSpells[2].Cast(spellSpawnArea);
                    shuffledSpells.RemoveAt(2);
                    StartCoroutine(SpellCooldown());
                }
                else
                {
                    return;
                }
            }
        }
    }

    private IEnumerator SpellCooldown()
    {
        isCooldown = true;
        currentCDTimer = 0;
        yield return new WaitForSeconds(spellCooldownDuration);
        isCooldown = false;
    }

    #region Inputs Enabler & Disabler
    private void OnEnable()
    {
        input1.Enable();
        input2.Enable();
        input3.Enable();
    }
    private void OnDisable()
    {
        input1.Disable();
        input2.Disable();
        input3.Disable();
    }
    #endregion
}
