using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using LaundromatManagementSystem.Services;
using System.Globalization;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class PaymentModalViewModel : ObservableObject
    {
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;

        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private string _transactionId = string.Empty;

        [ObservableProperty]
        private string _customer = string.Empty;

        [ObservableProperty]
        private string? _selectedMethod;

        [ObservableProperty]
        private string _cashReceived = "0";

        [ObservableProperty]
        private bool _processing = false;

        [ObservableProperty]
        private Language _language;

        [ObservableProperty]
        private Theme _theme;

        // Commands from parent
        public ICommand CloseCommand { get; }
        public ICommand PaymentCompleteCommand { get; }

        // Computed properties
        public decimal Change
        {
            get
            {
                if (decimal.TryParse(CashReceived, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal received) && received >= Total)
                {
                    return received - Total;
                }
                return 0;
            }
        }

        public bool CanCompletePayment
        {
            get
            {
                if (SelectedMethod == "Cash")
                {
                    return decimal.TryParse(CashReceived, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal received) && received >= Total;
                }
                return !string.IsNullOrEmpty(SelectedMethod);
            }
        }

        public bool IsMethodSelected => !string.IsNullOrEmpty(SelectedMethod);

        // Text properties
        public string Title => GetTranslation("payment");
        public string TransactionLabel => GetTranslation("transaction");
        public string TotalLabel => GetTranslation("total");
        public string CustomerLabel => GetTranslation("customer");
        public string CashLabel => GetTranslation("cash");
        public string MoMoLabel => GetTranslation("momo");
        public string CardLabel => GetTranslation("card");
        public string ReceivedLabel => GetTranslation("received");
        public string ChangeLabel => GetTranslation("change");
        public string CancelText => GetTranslation("cancel");
        public string ProcessButtonText => GetTranslation("process");
        public string SelectMethodLabel => GetTranslation("select_method");

        // Theme colors
        public Color BackgroundColor => GetBackgroundColor();
        public Color BorderColor => GetBorderColor();
        public Color TitleColor => GetTitleColor();
        public Color SubtitleColor => GetSubtitleColor();
        public Color TextColor => GetTextColor();
        public Color PlaceholderColor => GetPlaceholderColor();
        public Color InputBorderColor => GetInputBorderColor();
        public Color InputBackgroundColor => GetInputBackgroundColor();
        public Color AmountBorderColor => GetAmountBorderColor();
        public Color AmountBackgroundColor => GetAmountBackgroundColor();
        public Color ButtonBackgroundColor => GetButtonBackgroundColor();
        public Color ButtonTextColor => GetButtonTextColor();
        public Color CancelButtonBackgroundColor => GetCancelButtonBackgroundColor();
        public Color CancelButtonTextColor => GetCancelButtonTextColor();

        public PaymentModalViewModel(ICommand closeCommand, ICommand paymentCompleteCommand)
        {
            CloseCommand = closeCommand;
            PaymentCompleteCommand = paymentCompleteCommand;

            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;

            // Subscribe to state changes
            _stateService.PropertyChanged += OnStateChanged;
        }

        partial void OnLanguageChanged(Language value)
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(TransactionLabel));
            OnPropertyChanged(nameof(TotalLabel));
            OnPropertyChanged(nameof(CustomerLabel));
            OnPropertyChanged(nameof(CashLabel));
            OnPropertyChanged(nameof(MoMoLabel));
            OnPropertyChanged(nameof(CardLabel));
            OnPropertyChanged(nameof(ReceivedLabel));
            OnPropertyChanged(nameof(ChangeLabel));
            OnPropertyChanged(nameof(CancelText));
            OnPropertyChanged(nameof(ProcessButtonText));
            OnPropertyChanged(nameof(SelectMethodLabel));
        }

        partial void OnThemeChanged(Theme value)
        {
            UpdateThemeProperties();
        }

        [RelayCommand]
        private void SelectPaymentMethod(string method)
        {
            SelectedMethod = method;
            CashReceived = "0"; // Reset cash received when switching methods
            
            OnPropertyChanged(nameof(IsMethodSelected));
            OnPropertyChanged(nameof(CanCompletePayment));
            OnPropertyChanged(nameof(Change));
        }

        [RelayCommand]
        private void AddToCash(string input)
        {
            // Remove thousand separators for parsing
            var cleanCash = CashReceived.Replace(",", "");

            if (input == "Clear")
            {
                CashReceived = "0";
            }
            else if (input == "00")
            {
                if (cleanCash == "0")
                {
                    CashReceived = "0";
                }
                else
                {
                    cleanCash += "00";
                    if (long.TryParse(cleanCash, out long value))
                    {
                        CashReceived = value.ToString("N0", CultureInfo.InvariantCulture);
                    }
                }
            }
            else
            {
                if (cleanCash == "0")
                {
                    CashReceived = input;
                }
                else
                {
                    cleanCash += input;
                    if (long.TryParse(cleanCash, out long value))
                    {
                        CashReceived = value.ToString("N0", CultureInfo.InvariantCulture);
                    }
                }
            }

            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(CanCompletePayment));
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            if (!CanCompletePayment) return;

            Processing = true;
            
            try
            {
                // Simulate payment processing
                await Task.Delay(1000);
                
                // Create payment result
                var paymentResult = (
                    PaymentMethod: SelectedMethod switch
                    {
                        "Cash" => PaymentMethod.Cash,
                        "MoMo" => PaymentMethod.MoMo,
                        "Card" => PaymentMethod.Card,
                        _ => PaymentMethod.Cash
                    },
                    Customer: Customer
                );
                
                // Execute completion command
                if (PaymentCompleteCommand?.CanExecute(paymentResult) == true)
                {
                    PaymentCompleteCommand.Execute(paymentResult);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Payment processing failed: {ex.Message}", "OK");
            }
            finally
            {
                Processing = false;
            }
        }

        [RelayCommand]
        private void ClearSelection()
        {
            SelectedMethod = null;
            CashReceived = "0";
            
            OnPropertyChanged(nameof(IsMethodSelected));
            OnPropertyChanged(nameof(CanCompletePayment));
            OnPropertyChanged(nameof(Change));
        }

        [RelayCommand]
        private void ClosePaymentModal()
        {
            // Reset the modal state
            SelectedMethod = null;
            CashReceived = "0";
            Customer = string.Empty;
            
            // Update ApplicationStateService to close modal
            _stateService.ShowPaymentModal = false;
            
            // Also execute the parent close command if provided
            if (CloseCommand?.CanExecute(null) == true)
            {
                CloseCommand.Execute(null);
            }
        }

        private void OnStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_stateService.CurrentLanguage):
                        if (Language != _stateService.CurrentLanguage)
                        {
                            Language = _stateService.CurrentLanguage;
                        }
                        break;

                    case nameof(_stateService.CurrentTheme):
                        if (Theme != _stateService.CurrentTheme)
                        {
                            Theme = _stateService.CurrentTheme;
                        }
                        break;
                        
                    case nameof(_stateService.ShowPaymentModal):
                        // If state service says modal should be closed, execute close
                        if (!_stateService.ShowPaymentModal && CloseCommand?.CanExecute(null) == true)
                        {
                            CloseCommand.Execute(null);
                        }
                        break;
                }
            });
        }

        private void UpdateThemeProperties()
        {
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(TitleColor));
            OnPropertyChanged(nameof(SubtitleColor));
            OnPropertyChanged(nameof(TextColor));
            OnPropertyChanged(nameof(PlaceholderColor));
            OnPropertyChanged(nameof(InputBorderColor));
            OnPropertyChanged(nameof(InputBackgroundColor));
            OnPropertyChanged(nameof(AmountBorderColor));
            OnPropertyChanged(nameof(AmountBackgroundColor));
            OnPropertyChanged(nameof(ButtonBackgroundColor));
            OnPropertyChanged(nameof(ButtonTextColor));
            OnPropertyChanged(nameof(CancelButtonBackgroundColor));
            OnPropertyChanged(nameof(CancelButtonTextColor));
        }

        private string GetTranslation(string key)
        {
            var translations = new Dictionary<string, Dictionary<Language, string>>
            {
                ["payment"] = new()
                {
                    [Language.EN] = "Process Payment",
                    [Language.RW] = "Kwishyura",
                    [Language.FR] = "Traiter le Paiement"
                },
                ["transaction"] = new()
                {
                    [Language.EN] = "Transaction",
                    [Language.RW] = "Igicuruzwa",
                    [Language.FR] = "Transaction"
                },
                ["total"] = new()
                {
                    [Language.EN] = "Total",
                    [Language.RW] = "Igiteranyo",
                    [Language.FR] = "Total"
                },
                ["customer"] = new()
                {
                    [Language.EN] = "Customer (Optional)",
                    [Language.RW] = "Umukiriya (Sitegeko)",
                    [Language.FR] = "Client (Optionnel)"
                },
                ["cash"] = new()
                {
                    [Language.EN] = "Cash",
                    [Language.RW] = "Amafaranga",
                    [Language.FR] = "Espèces"
                },
                ["momo"] = new()
                {
                    [Language.EN] = "Mobile Money",
                    [Language.RW] = "Mobile Money",
                    [Language.FR] = "Mobile Money"
                },
                ["card"] = new()
                {
                    [Language.EN] = "Card",
                    [Language.RW] = "Ikarita",
                    [Language.FR] = "Carte"
                },
                ["received"] = new()
                {
                    [Language.EN] = "Cash Received",
                    [Language.RW] = "Amafaranga Yakiriwe",
                    [Language.FR] = "Espèces Reçues"
                },
                ["change"] = new()
                {
                    [Language.EN] = "Change Due",
                    [Language.RW] = "Amafaranga Asigaye",
                    [Language.FR] = "Monnaie à Rendre"
                },
                ["cancel"] = new()
                {
                    [Language.EN] = "CANCEL",
                    [Language.RW] = "HAGARIKA",
                    [Language.FR] = "ANNULER"
                },
                ["process"] = new()
                {
                    [Language.EN] = "COMPLETE PAYMENT",
                    [Language.RW] = "GUSOZA KWISHYURA",
                    [Language.FR] = "TERMINER LE PAIEMENT"
                },
                ["select_method"] = new()
                {
                    [Language.EN] = "Select Payment Method",
                    [Language.RW] = "Hitamo Uburyo bwo Kwishyura",
                    [Language.FR] = "Sélectionner le Mode de Paiement"
                }
            };

            return translations.ContainsKey(key) ? translations[key][Language] : key;
        }

        private Color GetBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#1F2937"),
                Theme.Gray => Colors.White,
                _ => Colors.White
            };
        }

        private Color GetBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                _ => Color.FromArgb("#E5E7EB")
            };
        }

        private Color GetTitleColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Color.FromArgb("#1E3A8A")
            };
        }

        private Color GetSubtitleColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#9CA3AF"),
                _ => Color.FromArgb("#6B7280")
            };
        }

        private Color GetTextColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Color.FromArgb("#111827")
            };
        }

        private Color GetPlaceholderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#6B7280"),
                _ => Color.FromArgb("#9CA3AF")
            };
        }

        private Color GetInputBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                _ => Color.FromArgb("#D1D5DB")
            };
        }

        private Color GetInputBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#111827"),
                _ => Color.FromArgb("#D1D5DB")
            };
        }

        private Color GetAmountBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                _ => Color.FromArgb("#E5E7EB")
            };
        }

        private Color GetAmountBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#111827"),
                _ => Color.FromArgb("#F9FAFB")
            };
        }

        private Color GetButtonBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                _ => Color.FromArgb("#F3F4F6")
            };
        }

        private Color GetButtonTextColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Color.FromArgb("#111827")
            };
        }

        private Color GetCancelButtonBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                _ => Color.FromArgb("#F3F4F6")
            };
        }

        private Color GetCancelButtonTextColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Color.FromArgb("#111827")
            };
        }

        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;
        }
    }
}