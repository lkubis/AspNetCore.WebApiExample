namespace AspNetCore.Domain
{
    public class Product
    {
        private int _id;

        public int Id => _id;
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgUri { get; set; }
        public decimal Price { get; set; }
    }
}
