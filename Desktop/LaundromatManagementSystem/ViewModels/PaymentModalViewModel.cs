using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using LaundromatManagementSystem.Services;
using System.Globalization;
using System.Collections.ObjectModel;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class PaymentModalViewModel : ObservableObject
    {
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;

        [ObservableProperty]
        private decimal _total;

        private string _transactionId = string.Empty;

        [ObservableProperty]
        private string _customer = string.Empty;

        [ObservableProperty]
        private string? _selectedMethod;

        [ObservableProperty]
        private string _cashReceived = string.Empty;

        [ObservableProperty]
        private bool _processing = false;

        [ObservableProperty]
        private Language _language;

        [ObservableProperty]
        private Theme _theme;

        [ObservableProperty]
        private ObservableCollection<CartItem> _cartItems = new();

        // Commands from parent
        public ICommand CloseCommand { get; }
        public ICommand PaymentCompleteCommand { get; }

        // Computed properties
        public decimal Change
        {
            get
            {
                if (SelectedMethod == "Cash")
                {
                    return 0;
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
                    return decimal.TryParse(CashReceived.Replace(",", "").Replace(".", ""),
                        NumberStyles.Number, CultureInfo.InvariantCulture, out decimal received) && received >= Total;
                }
                // For MoMo and Card, we just need a method selected
                return !string.IsNullOrEmpty(SelectedMethod);
            }
        }

        public bool IsMethodSelected => !string.IsNullOrEmpty(SelectedMethod);

        // Text properties
        public string Title => GetTranslation("title");
        public string TransactionLabel => GetTranslation("transaction");
        public string TotalLabel => GetTranslation("total");
        public string CustomerLabel => GetTranslation("customer");
        public string CashLabel => GetTranslation("cash");
        public string MoMoLabel => GetTranslation("momo");
        public string CardLabel => GetTranslation("card");
        public string ReceivedLabel => GetTranslation("received");
        public string ChangeLabel => GetTranslation("change");
        public string CancelText => GetTranslation("cancel");
        public string SelectMethodLabel => GetTranslation("select_method");
        public string ProcessButtonText => GetProcessingButtonText();

        [ObservableProperty]
        private decimal _subtotal;

        [ObservableProperty]
        private decimal _tax;

        [ObservableProperty]
        private decimal _grandTotal;

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

        private IPrinterService? _printerService;

        public PaymentModalViewModel(ICommand closeCommand, ICommand paymentCompleteCommand)
        {
            CloseCommand = closeCommand;
            PaymentCompleteCommand = paymentCompleteCommand;

            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;

            Subtotal = _stateService.CartTotal;
            Tax = Subtotal * 0.1m;
            GrandTotal = Subtotal + Tax;

            // Initialize cart items and total
            Total = Subtotal + Tax;
            CartItems = new ObservableCollection<CartItem>(_stateService.CartItems);

            // Subscribe to state changes
            _stateService.PropertyChanged += OnStateChanged;

            try
            {
                _printerService = ServiceLocator.GetService<IPrinterService>();
            }
            catch (Exception ex)
            {
                // Handle missing printer service
                _printerService = null;
            }
        }

        partial void OnLanguageChanged(Language value)
        {
            UpdateTextProperties();
        }

        partial void OnThemeChanged(Theme value)
        {
            UpdateThemeProperties();
        }

        public string TransactionId
        {
            get
            {
                if (string.IsNullOrEmpty(_transactionId))
                {
                    _transactionId = _stateService.GenerateTransactionId();
                }
                return _transactionId;
            }
            set => SetProperty(ref _transactionId, value);
        }

        [RelayCommand]
        private void SelectPaymentMethod(string method)
        {
            SelectedMethod = method;
            CashReceived = Total.ToString("N0", CultureInfo.InvariantCulture);

            OnPropertyChanged(nameof(IsMethodSelected));
            OnPropertyChanged(nameof(CanCompletePayment));
            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(ProcessButtonText));
        }

        [RelayCommand]
        private void AddToCash(string input)
        {
            if (input == "Clear")
            {
                CashReceived = string.Empty;
            }
            else if (input == "00")
            {
                if (string.IsNullOrEmpty(CashReceived))
                {
                    CashReceived = "0";
                }
                else
                {
                    CashReceived += "00";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(CashReceived))
                {
                    CashReceived = input;
                }
                else
                {
                    CashReceived += input;
                }
            }

            // Format the number if it's a valid number
            if (long.TryParse(CashReceived.Replace(",", "").Replace(".", ""), out long value))
            {
                CashReceived = value.ToString("N0", CultureInfo.InvariantCulture);
            }

            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(CanCompletePayment));
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            if (!CanCompletePayment) return;

            if (SelectedMethod == "Cash")
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Confirm Cash Payment",
                    $"You are about to process a cash payment of {Total:N0} RWF.\n" +
                    $"Cash Received: {CashReceived}\n" +
                    $"Change Due: {Change:N0} RWF",
                    "OK"
                );
            }

            Processing = true;

            try
            {
                // Generate new transaction ID
                var newTransactionId = TransactionId;

                // Create payment result
                var paymentResult = new PaymentResult
                {
                    PaymentMethod = SelectedMethod switch
                    {
                        "Cash" => PaymentMethod.Cash,
                        "MoMo" => PaymentMethod.MoMo,
                        "Card" => PaymentMethod.Card,
                        _ => PaymentMethod.Cash
                    },
                    Customer = Customer,
                    Subtotal = Subtotal,
                    Tax = Tax,
                    GrandTotal = Subtotal + Tax,
                    Amount = Total,
                    Change = Change,
                    TransactionId = newTransactionId,
                    Items = CartItems.ToList()
                };

                // Print receipt with all details
                await PrintReceipt(paymentResult);

                // Execute completion command
                if (PaymentCompleteCommand?.CanExecute(paymentResult) == true)
                {
                    PaymentCompleteCommand.Execute(paymentResult);
                }

                // Show success message
                await Application.Current.MainPage.DisplayAlert(
                    GetTranslation("success"),
                    $"Transaction: {newTransactionId}\n" +
                    $"Subtotal: {Subtotal:N0} RWF\n" +
                    $"Tax: {Tax:N0} RWF\n" +
                    $"Grand Total: {GrandTotal:N0} RWF\n" +
                    $"Customer: {(string.IsNullOrEmpty(Customer) ? "N/A" : Customer)}\n" +
                    $"Payment Method: {SelectedMethod}\n" +
                    $"{(SelectedMethod == "Cash" ? $"Cash Received: {CashReceived}\nChange: {Change:N0} RWF" : "")}",
                    "OK");

                // Clear cart using the public method
                ClearCart();

                // Close modal after successful payment
                ClosePaymentModal();

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

        private void RecalculateTotals()
        {
            Subtotal = _stateService.CartTotal;
            Tax = Subtotal * 0.1m;
            GrandTotal = Subtotal + Tax;
        }


        private void ClearCart()
        {
            SelectedMethod = null;
            CashReceived = string.Empty;
            // Use the public method to clear cart
            _stateService.ClearCart();

            // Update the local cart items
            CartItems.Clear();

            // Update total
            Total = 0;
            OnPropertyChanged(nameof(Total));
        }

        private async Task PrintReceipt(PaymentResult paymentInfo)
        {
            try
            {
                var receiptContent = new ReceiptContent
                {
                    TransactionId = paymentInfo.TransactionId,
                    CustomerPhone = paymentInfo.Customer,
                    PaymentMethod = paymentInfo.PaymentMethod.ToString(),
                    Subtotal = paymentInfo.Subtotal,
                    Tax = paymentInfo.Tax,
                    GrandTotal = paymentInfo.GrandTotal,
                    Amount = paymentInfo.Amount,
                    Change = paymentInfo.Change,
                    Items = paymentInfo.Items,
                    Date = DateTime.Now,
                    Cashier = "System"
                };

                if (_printerService != null)
                {
                    await _printerService.PrintReceipt(receiptContent);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Printer Unavailable",
                        "Receipt printing service is not available.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                // Show warning but continue
                await Application.Current.MainPage.DisplayAlert(
                    "Print Warning",
                    "Payment was successful but receipt printing failed. Transaction has been recorded.",
                    "OK");
            }
        }

        [RelayCommand]
        private void ClearSelection()
        {
            SelectedMethod = null;
            CashReceived = string.Empty;

            OnPropertyChanged(nameof(IsMethodSelected));
            OnPropertyChanged(nameof(CanCompletePayment));
            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(ProcessButtonText));
        }

        [RelayCommand]
        private void ClosePaymentModal()
        {
            // Reset the modal state
            SelectedMethod = null;
            CashReceived = string.Empty;
            Customer = string.Empty;

            // Update ApplicationStateService to close modal
            _stateService.ShowPaymentModal = false;

            // Also execute the parent close command if provided
            if (CloseCommand?.CanExecute(null) == true)
            {
                CloseCommand.Execute(null);
            }
        }

        private string GetProcessingButtonText()
        {
            if (Processing) return GetTranslation("processing");

            return SelectedMethod switch
            {
                "Cash" => GetTranslation("process"),
                "MoMo" => GetTranslation("generateQR"),
                "Card" => GetTranslation("processCard"),
                _ => GetTranslation("confirm")
            };
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

                    case nameof(_stateService.CartItems):
                        // Update local cart items when state service cart changes
                        CartItems = new ObservableCollection<CartItem>(_stateService.CartItems);
                        Total = _stateService.CartTotal;

                        RecalculateTotals();

                        OnPropertyChanged(nameof(CartItems));
                        OnPropertyChanged(nameof(Total));
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

        private void UpdateTextProperties()
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
                ["title"] = new()
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
                    [Language.EN] = "Phone Number or TIN",
                    [Language.RW] = "Nimero ya Telefoni cyangwa TIN",
                    [Language.FR] = "Numéro de Téléphone ou TIN"
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
                ["received"] = new()
                {
                    [Language.EN] = "Cash Received",
                    [Language.RW] = "Amafaranga Yakiriwe",
                    [Language.FR] = "Espèces Reçues"
                },
                ["change"] = new()
                {
                    [Language.EN] = "Change Due",
                    [Language.RW] = "Amafaranga Yo Gusubiza",
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
                    [Language.EN] = "Process Cash",
                    [Language.RW] = "Emeza Amafaranga",
                    [Language.FR] = "Traiter Espèces"
                },
                ["generateQR"] = new()
                {
                    [Language.EN] = "Generate QR Code",
                    [Language.RW] = "Kora QR Code",
                    [Language.FR] = "Générer QR Code"
                },
                ["confirm"] = new()
                {
                    [Language.EN] = "Confirm Payment",
                    [Language.RW] = "Emeza Kwishyura",
                    [Language.FR] = "Confirmer Paiement"
                },
                ["swipeCard"] = new()
                {
                    [Language.EN] = "Swipe/Tap Card",
                    [Language.RW] = "Koresha Karita",
                    [Language.FR] = "Glisser/Taper Carte"
                },
                ["processCard"] = new()
                {
                    [Language.EN] = "Process Card",
                    [Language.RW] = "Emeza Karita",
                    [Language.FR] = "Traiter Carte"
                },
                ["processing"] = new()
                {
                    [Language.EN] = "Processing...",
                    [Language.RW] = "Gutunganya...",
                    [Language.FR] = "Traitement..."
                },
                ["success"] = new()
                {
                    [Language.EN] = "Payment Successful!",
                    [Language.RW] = "Kwishyura Byagenze Neza!",
                    [Language.FR] = "Paiement Réussi!"
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

        // Enhanced class for payment result
        public class PaymentResult
        {
            public PaymentMethod PaymentMethod { get; set; }
            public string Customer { get; set; } = string.Empty;
            public decimal Subtotal { get; set; }
            public decimal Tax { get; set; }
            public decimal GrandTotal { get; set; }
            public decimal Amount { get; set; }
            public decimal Change { get; set; }
            public string TransactionId { get; set; } = string.Empty;
            public List<CartItem> Items { get; set; } = new();
        }
    }
}