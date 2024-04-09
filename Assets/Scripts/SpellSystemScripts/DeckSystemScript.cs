using System;
using System.Collections;
using System.Collections.Generic;
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

    int spellCooldownDuration = 2;
    private bool isCooldown = false;

    [Header("Object References")]
    public GameObject cardSlotOne;
    public GameObject cardSlotTwo;
    public GameObject cardSlotThree;
    public Image csOne;
    public Image csTwo;
    public Image csThree;

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

        public Spell(Sprite sprite)
        {
            spellSprite = sprite;
        }

        public virtual void Cast(GameObject spawnPoint) { }
        public virtual Sprite SpellSprite
        {
            get { return spellSprite; }
        }
    }
    #endregion

    #region Unique Spell Classes
    public class FireballSpell : Spell
    {
        public GameObject fireballPrefab;

        public FireballSpell(Sprite sprite) : base(sprite)
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

        public LightningStrikeSpell(Sprite sprite) : base(sprite)
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

        public ToxicCloudSpell(Sprite sprite) : base(sprite)
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

        public RevitilizeSpell(Sprite sprite) : base(sprite)
        {
        }

        public override void Cast(GameObject spawnPoint)
        {
            playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            playerHP.ps.health += 10f;
        }
    }

    public class WarpedWorldSpell: Spell
    {
        public Camera wwCam;
        public GameObject warpedWorldPrefab;

        public WarpedWorldSpell(Sprite sprite) : base(sprite)
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
        totalSpells.Add(lightningStrikeSpell);
        totalSpells.Add(toxicCloudSpell);
        totalSpells.Add(lightningStrikeSpell);
        totalSpells.Add(fireballSpell);
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
                        break;
                    case 1:
                        cardSlotTwo.SetActive(true);
                        csTwo.sprite = shuffledSpells[i].SpellSprite;
                        break;
                    case 2:
                        cardSlotThree.SetActive(true);
                        csThree.sprite = shuffledSpells[i].SpellSprite;
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
        fireballSpell = new FireballSpell(fireBallSprite);
        fireballSpell.fireballPrefab = fireBallPrefab;

        lightningStrikeSpell = new LightningStrikeSpell(lightningStrikeSprite);
        lightningStrikeSpell.lsCam = cam;
        lightningStrikeSpell.lightningStrikePrefab = lightningStrikePrefab;

        toxicCloudSpell = new ToxicCloudSpell(toxicCloudSprite);
        toxicCloudSpell.tcCam = cam;
        toxicCloudSpell.toxicCloudPrefab = toxicCloudPrefab;

        revitilizeSpell = new RevitilizeSpell(revitilizeSprite);

        warpedWorldSpell = new WarpedWorldSpell(warpedWorldSprite);
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
