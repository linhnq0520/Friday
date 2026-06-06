using Friday.Modules.Salon.Domain.Enums;

namespace Friday.Modules.Salon.Application.Models;

public static class GalleryCategoryInfo
{
    public static readonly GalleryCategory[] CollectionCategories =
    [
        GalleryCategory.FashionColor,
        GalleryCategory.TrendingStyle,
        GalleryCategory.HairRecovery,
        GalleryCategory.HairExtensions,
    ];

    public static string GetFolderSlug(GalleryCategory category) =>
        category switch
        {
            GalleryCategory.FashionColor => "mau_thoi_trang",
            GalleryCategory.TrendingStyle => "kieu_toc_thinh_hanh",
            GalleryCategory.HairRecovery => "phuc_hoi_hu_ton",
            GalleryCategory.HairExtensions => "noi_toc",
            _ => category.ToString().ToLowerInvariant(),
        };

    public static GalleryCategory? FromFolderSlug(string slug) =>
        slug.ToLowerInvariant() switch
        {
            "mau_thoi_trang" => GalleryCategory.FashionColor,
            "kieu_toc_thinh_hanh" => GalleryCategory.TrendingStyle,
            "phuc_hoi_hu_ton" => GalleryCategory.HairRecovery,
            "noi_toc" => GalleryCategory.HairExtensions,
            _ => null,
        };

    public static string GetDefaultCover(GalleryCategory category) =>
        category switch
        {
            GalleryCategory.FashionColor => "/resources/khong_gian/khong-gian14.jpg",
            GalleryCategory.TrendingStyle => "/resources/khong_gian/khong-gian20.jpg",
            GalleryCategory.HairRecovery => "/resources/khong_gian/khong-gian22.jpg",
            GalleryCategory.HairExtensions => "/resources/khong_gian/khong-gian28.jpg",
            _ => "/resources/khong_gian/khong-gian10.jpg",
        };
}
