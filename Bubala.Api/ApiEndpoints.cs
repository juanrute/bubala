namespace Bubala.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Product
    {
        private const string Base = $"{ApiBase}/products";

        public const string Create = Base;
        public const string Get = $"{Base}/{{idOrSlug}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Review = $"{Base}/{{id:guid}}/reviews";
        public const string DeleteReview = $"{Base}/{{id:guid}}/reviews";
    }

    public static class Reviews
    {
        public const string Base = $"{ApiBase}/reviews";
        public const string GetUserReviews = $"{Base}/me";
    }
}