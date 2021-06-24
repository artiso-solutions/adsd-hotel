namespace artiso.AdsdHotel.Red.Ambassador
{
    public static class RedAmbassadorFactory
    {
        public static RedAmbassador Create()
        {
            return new("http://localhost:5001");
        }
    }
}
