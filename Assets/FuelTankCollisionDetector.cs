using System;
using LiveLarson.SoundSystem;
using UnityEngine;

public class FuelTankCollisionDetector : MonoBehaviour
{
    [SerializeField] private GameObject fuelTank;
    [SerializeField] private string sfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            Debug.Log("[FuelTankCollisionDetector] Player collided with Fuel Tank");
            SoundService.PlaySfx(sfx, transform.position);
            Destroy(fuelTank);
            FuelCountPopupUI.ShowIncrease();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            Debug.Log("[FuelTankCollisionDetector] Player collided with Fuel Tank");
            SoundService.PlaySfx(sfx, transform.position);
            Destroy(fuelTank);
            FuelCountPopupUI.ShowIncrease();
        }
    }
}