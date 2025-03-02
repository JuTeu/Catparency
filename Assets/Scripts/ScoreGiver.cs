using UnityEngine;

namespace Catparency
{
    public class ScoreGiver : MonoBehaviour
    {
        public void GiveScore(int score)
        {
            ScoreCounter.Score += score;
        }
    }
}
