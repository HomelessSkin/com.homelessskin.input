using System;

using UnityEngine;

namespace Input
{
    public class Command : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] bool SkipValidation;
#endif
        [SerializeField] bool SkipCreation;
        [SerializeField] bool SkipRemovingOnDestroy;
        [SerializeField] Reward Reward;

        int Calls = 0;
        int Index;

        public void AddCall(int index)
        {
            Calls++;
            Index = index;
        }
        public void SetReward(Reward reward) => Reward = reward;
        public int GetCalls(out int index)
        {
            index = Index;

            return Calls;
        }

        public bool TryGetReward(out Reward reward, bool onInit = true)
        {
            reward = Reward;

            return onInit ? !SkipCreation : !SkipRemovingOnDestroy;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (Reward == null)
                Reward = new Reward();

            if (!SkipValidation)
                Reward.title = name;
        }
#endif
    }

    [Serializable]
    public class Reward
    {
        public int cost;
        public int max_per_stream;
        public string id;
        public string title;
        public string prompt;
        public string user_input;
        public bool should_stay_on_exit;
        public bool is_user_input_required;
        public bool is_max_per_stream_enabled;
        public bool should_redemptions_skip_request_queue;
    }
}