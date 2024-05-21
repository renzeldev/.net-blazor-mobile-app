namespace UmfaApp.Models
{
    public class Partner
    {

        public int PartnerId { get; set; }
        public string Name { get; set; }

        public bool IsActivePartner { get; set; }

        public Partner() { }
        public Partner(Data.Tables.Partner partner)
        {
            PartnerId = partner.PartnerId;
            Name = partner.Name;
            IsActivePartner = true;
        }
    }
}
