using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class PaymentModalViewModel : ObservableObject
    {
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;

        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private string _transactionId = string.Empty;

        // Keep settable properties for XAML binding
        [ObservableProperty]
        private Language _language;

        [ObservableProperty]
        private Theme _theme;

        [ObservableProperty]
        private PaymentMethod? _selectedMethod;

        [ObservableProperty]
        private string _cashReceived = string.Empty;

        [ObservableProperty]
        private string _customer = string.Empty;

        [ObservableProperty]
        private bool _isProcessing;

        // Text properties
        public string Title => GetTranslation("title");
        public string TransactionLabel => GetTranslation("transaction");
        public string TotalLabel => GetTranslation("total");
        public string CustomerLabel => GetTranslation("customer");
        public string CashLabel => GetTranslation("cash");
        public string MoMoLabel => GetTranslation("momo");
        public string CardLabel => GetTranslation("card");
        public string ProcessCashText => GetTranslation("process");
        public string ConfirmPaymentText => GetTranslation("confirm");
        public string CancelText => GetTranslation("cancel");
        public string ProcessingText => GetTranslation("processing");

        // Computed properties
        public decimal Change => !string.IsNullOrEmpty(CashReceived) &&
                                decimal.TryParse(CashReceived, out var received)
                                ? Math.Max(0, received - Total)
                                : 0;

        public bool CanProcessCash => !string.IsNullOrEmpty(CashReceived) &&
                                     decimal.TryParse(CashReceived, out var received) &&
                                     received >= Total;

        public bool CanCompletePayment => SelectedMethod.HasValue &&
                                         (SelectedMethod != PaymentMethod.Cash || CanProcessCash);

        public ICommand CloseCommand { get; }
        public ICommand PaymentCompleteCommand { get; }

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

        // Override setters to update state service
        partial void OnLanguageChanged(Language value)
        {
            if (_stateService.CurrentLanguage != value)
            {
                _stateService.CurrentLanguage = value;
            }
            UpdateTextProperties();
        }

        partial void OnThemeChanged(Theme value)
        {
            if (_stateService.CurrentTheme != value)
            {
                _stateService.CurrentTheme = value;
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
                }
            });
        }
        private void UpdateTextProperties()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(TransactionLabel));
            OnPropertyChanged(nameof(TotalLabel));
            OnPropertyChanged(nameof(CustomerLabel));
            OnPropertyChanged(nameof(CashLabel));
            OnPropertyChanged(nameof(MoMoLabel));
            OnPropertyChanged(nameof(CardLabel));
            OnPropertyChanged(nameof(ProcessCashText));
            OnPropertyChanged(nameof(ConfirmPaymentText));
            OnPropertyChanged(nameof(CancelText));
            OnPropertyChanged(nameof(ProcessingText));
        }

        [RelayCommand]
        private void SelectPaymentMethod(string method)
        {
            SelectedMethod = method switch
            {
                "Cash" => PaymentMethod.Cash,
                "MoMo" => PaymentMethod.MoMo,
                "Card" => PaymentMethod.Card,
                _ => null
            };
            OnPropertyChanged(nameof(CanCompletePayment));
        }

        [RelayCommand]
        private void ClearSelection()
        {
            SelectedMethod = null;
            CashReceived = string.Empty;
            OnPropertyChanged(nameof(CanCompletePayment));
        }

        [RelayCommand]
        private void AddToCash(string digit)
        {
            if (digit == "Clear")
            {
                CashReceived = string.Empty;
            }
            else
            {
                CashReceived += digit;
            }

            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(CanProcessCash));
            OnPropertyChanged(nameof(CanCompletePayment));
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            if (!CanCompletePayment || !SelectedMethod.HasValue)
                return;

            IsProcessing = true;

            // Simulate payment processing
            await Task.Delay(1500);

            PaymentCompleteCommand?.Execute((SelectedMethod.Value, Customer));

            IsProcessing = false;
            ClearSelection();
        }

        private string GetTranslation(string key)
        {
            var translations = new Dictionary<string, Dictionary<Language, string>>
            {
                ["title"] = new()
                {
                    [Language.EN] = "Payment Processing",
                    [Language.RW] = "Gutanga Amafaranga",
                    [Language.FR] = "Traitement du Paiement"
                },
                ["transaction"] = new()
                {
                    [Language.EN] = "Transaction #",
                    [Language.RW] = "Ikirangirizo #",
                    [Language.FR] = "Transaction #"
                },
                ["total"] = new()
                {
                    [Language.EN] = "Total",
                    [Language.RW] = "Igiteranyo",
                    [Language.FR] = "Total"
                },
                ["customer"] = new()
                {
                    [Language.EN] = "Customer",
                    [Language.RW] = "Umukiriya",
                    [Language.FR] = "Client"
                },
                ["cash"] = new()
                {
                    [Language.EN] = "CASH",
                    [Language.RW] = "AMAFARANGA",
                    [Language.FR] = "ESPÈCES"
                },
                ["momo"] = new()
                {
                    [Language.EN] = "MOBILE MONEY",
                    [Language.RW] = "MOMO",
                    [Language.FR] = "MOBILE MONEY"
                },
                ["card"] = new()
                {
                    [Language.EN] = "CARD PAYMENT",
                    [Language.RW] = "KARITA",
                    [Language.FR] = "PAIEMENT CARTE"
                },
                ["process"] = new()
                {
                    [Language.EN] = "Process Cash",
                    [Language.RW] = "Emeza Amafaranga",
                    [Language.FR] = "Traiter Espèces"
                },
                ["confirm"] = new()
                {
                    [Language.EN] = "Confirm Payment",
                    [Language.RW] = "Emeza Kwishyura",
                    [Language.FR] = "Confirmer Paiement"
                },
                ["cancel"] = new()
                {
                    [Language.EN] = "Cancel",
                    [Language.RW] = "Hagarika",
                    [Language.FR] = "Annuler"
                },
                ["processing"] = new()
                {
                    [Language.EN] = "Processing...",
                    [Language.RW] = "Gutunganya...",
                    [Language.FR] = "Traitement..."
                }
            };

            return translations[key][Language];
        }

        // Clean up
        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;
        }
    }
}