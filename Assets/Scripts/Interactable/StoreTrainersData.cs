using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoreTrainersData : MonoBehaviour
{
    public TrainerController[] storedTrainers;
    Dictionary<string, bool> trainersBattleLost = new Dictionary<string, bool>();


    public static StoreTrainersData Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        storedTrainers = FindObjectsOfType<TrainerController>();

        Instance = this;
    }

    public void SetTrainerBattleLost(string trainerName, bool lost)
    {
        trainersBattleLost[trainerName] = lost;
    }

    public bool HasTrainerLostBattle(string trainerName)
    {
        return trainersBattleLost.ContainsKey(trainerName) ? trainersBattleLost[trainerName] : false;
    }

    public void ExecuteChecking()
    {
        storedTrainers = FindObjectsOfType<TrainerController>();

        foreach (var trainer in trainersBattleLost)
        {
            if (HasTrainerLostBattle(trainer.Key))
            {
                foreach (var storedTrainer in storedTrainers)
                {
                    if (storedTrainer.TrainerName == trainer.Key)
                    {
                        storedTrainer.BattleLost = true;
                        storedTrainer.Fov.SetActive(false);
                    }
                }
            }
        }
    }
}
