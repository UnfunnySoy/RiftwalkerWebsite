namespace RiftwalkerWebsite.APIModels
{
    public class GameRunAPIModel
    {
        public Guid user_id { get; set; }
        public int highest_round { get; set; }
        public int total_coins { get; set; }
        public string character_class { get; set; }
        public long run_timestamp { get; set; }
    }
}
