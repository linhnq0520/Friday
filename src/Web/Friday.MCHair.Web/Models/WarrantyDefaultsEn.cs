namespace Friday.MCHair.Web.Models;

public static class WarrantyDefaultsEn
{
    public const string WarrantyPeriodsBody =
        "<strong>Color:</strong> 30-day warranty.\n<strong>Perm / straightening:</strong> 60-day warranty.\n<strong>Repair:</strong> 30-day warranty.\n<strong>Haircut:</strong> 15-day warranty.";

    public static WarrantyPageData Create() =>
        new()
        {
            Title = "Warranty policy",
            Lead =
                "We are committed to service quality and client satisfaction after every visit.",
            MetaDescription =
                "MC Hair Salon warranty policy for hair services – quality commitment and client satisfaction.",
            Sections =
            [
                new()
                {
                    Title = "1. Warranty scope",
                    Format = "paragraph",
                    Body =
                        "MC Hair offers warranty for color, perm, straightening and repair services within the stated period for each service type. Warranty applies when products and techniques follow MC Hair's standard procedures.",
                },
                new()
                {
                    Title = "2. Warranty period",
                    Format = "list",
                    Body = WarrantyPeriodsBody,
                },
                new()
                {
                    Title = "3. Conditions",
                    Format = "list",
                    Body =
                        "Keep your receipt or booking confirmation from MC Hair.\nFollow post-service care instructions from your stylist.\nDo not use chemicals, color or excessive heat elsewhere during the warranty period.\nContact the salon before visiting for adjustment appointments.",
                },
                new()
                {
                    Title = "4. Exclusions",
                    Format = "list",
                    Body =
                        "Severe damage from improper home care after service.\nRequests for a completely different style/color from the original service.\nBeyond the warranty period communicated at service time.",
                },
                new()
                {
                    Title = "5. Support",
                    Format = "paragraph",
                    Body =
                        "For warranty questions, call hotline <strong id=\"warranty-hotline\">0988305371</strong> or message us on <a href=\"https://www.facebook.com/profile.php?id=61551835762411\" target=\"_blank\" rel=\"noopener\">MC Hair fanpage</a>.",
                },
            ],
        };
}
