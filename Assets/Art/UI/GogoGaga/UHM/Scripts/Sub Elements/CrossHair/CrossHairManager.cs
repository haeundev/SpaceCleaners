using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GogoGaga.UHM
{
    public class CrossHairManager : MonoBehaviour
    {
        public CrosshairGraphics[] CrossHairs;

        public Transform CrossHairParent;

        Animator crossHairAnim;
        CrosshairGraphics crosshairGraphics;

        public void Create(int type)
        {
            if (CrossHairs.Length == 0)
            {
                Debug.Log("No cross hair in array");
                return;
            }

            if (type < 0 || type >= CrossHairs.Length)
            {
                Debug.Log("There are " + CrossHairs.Length + " cross hairs." + type + " is not in range so putting close on");
                type = Mathf.Clamp(type, 0, CrossHairs.Length - 1);
            }

            Create(CrossHairs[type]);
        }

        public void Create(CrosshairGraphics newCrosshair)
        {
            foreach (Transform item in CrossHairParent)
            {
                Destroy(item.gameObject);
            }

            crosshairGraphics = Instantiate(newCrosshair, CrossHairParent.position, Quaternion.identity, CrossHairParent);
            crossHairAnim = crosshairGraphics.GetComponent<Animator>();

            Show();
        }


        public void SetColor(Color newColor)
        {
            if (crosshairGraphics != null)
                crosshairGraphics.SetColor(newColor);
        }


        public void SetHitColor(Color newColor)
        {
            if (crosshairGraphics != null)
                crosshairGraphics.SetHitColor(newColor);
        }

        public void Show()
        {

            StartCoroutine(PlayAnim("Show"));
        }

        public void Idle()
        {
            StartCoroutine(PlayAnim("Idle"));
        }

        public void HitEffect()
        {
            StartCoroutine(PlayAnim("Hit"));
        }


        public void ShootEffect()
        {
            StartCoroutine(PlayAnim("Shoot"));
        }

        public void HighlightEffect()
        {
            StartCoroutine(PlayAnim("Highlight"));
        }

        Coroutine hideCorotuine;
        public void Hide()
        {
            hideCorotuine = StartCoroutine(Hiding());
        }

        IEnumerator Hiding()
        {
            yield return new WaitForEndOfFrame();
            crossHairAnim.Play("Hide");

            yield return new WaitForSeconds(0.3f);
            gameObject.SetActive(false);
        }


        IEnumerator PlayAnim(string _name)
        {
            if (crossHairAnim == null)
            {
                Debug.Log("Please create a crosshair first");
                yield break;
            }

            if (hideCorotuine != null)
                StopCoroutine(hideCorotuine);

            crossHairAnim.enabled = false;

            yield return new WaitForEndOfFrame();

            crossHairAnim.enabled = true;
            crossHairAnim.Play(_name);
        }

    }
}