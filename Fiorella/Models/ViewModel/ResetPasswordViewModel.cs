using System.ComponentModel.DataAnnotations;

namespace Fiorella.Models.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; } =
            "https://localhost:5001/Account/ResetPassword?email=a@code.az&token=CfDJ8Lm-cvM8FuJEkBN32uVg1t0qUh51l1sTjTH4cmqtBHhVUnixRRQm0FRul047hzxwY58RM2MFbzmDNa34DV8UcTLc1Vok0dnIJkLhJl03wabfF49norUE6JhDyTsDYBGQw-DLtxIHM8IZVON2-c78NxzUG8JpmzCW7lyECYxxfbhI4i4s5XMNmSKH1m7F3kVi3gU2YIHWOWMWF5ifzeKLGr1ASjTnRnlrWPg7YS_OtqrHCexboD2BFSJoqNplYZ7wi7Qh6fzV_-QHtsataCcMvKFHKRNSAA9mfoIkhKiKfU0S5xoyPC0RX8sYe4oDK5VAjTrsTSvOdkKRH4Ccpa2MQPVtQJ007fewLSLYlZ-qfroXabBOxERubb5fgwRQiElOzAjsbmCrNGgzaOyZKitWR42sLn2Fj_IgXkRR6mUK4_Co4wmKalN7X_c5ivxL5JJRfmRcMVJmFlMFGCU8u-wecrLwLCPVdwboOxpFyvloGVfPfixlSHcR-5r7fLV3Xem0GQVnzvvH3xlw2dWy9BQqBR0j8aDq22akwG9n8DlpW7wvFLdgKD0WUSv0a7I2KYcbnxZZ03WMQDOLzCULzE1HEqoqHMfgZjQVlV9_sQDCwnkE_363Tu9c9gI-gJBH4xl5XntPGSQ3YOEds9X3_RGp1Io5Vzj-i0lakQohBffMZ5tq_TwHVNnGQcLHTG-L00ZZNw2S4PUD1MaAnU7UjccvrFjc0EqdHCqBdqUCHUS4sJg-";
    }
}