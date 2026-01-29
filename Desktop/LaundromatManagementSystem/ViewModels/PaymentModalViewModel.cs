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

        [ObservableProperty]
        private bool _showPaymentMethods = false;

        private string _transactionId = string.Empty;

        [ObservableProperty]
        private string _customer = string.Empty;

        [ObservableProperty]
        private string _tinNumber = string.Empty;

        [ObservableProperty]
        private string _purchaseCode = string.Empty;

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
        public string CustomerInfoTitle => GetTranslation("customerInfoTitle");
        public string TinLabel => GetTranslation("tin");
        public string OrLabel => GetTranslation("or");
        public string ClearTinButtonText => GetTranslation("clear");
        public string PurchaseCodeLabel => GetTranslation("purchaseCode");
        public string PurchaseCodeAlert => GetTranslation("purchaseCodeAlert");
        public string CustomerValidationSuccessMessage => GetTranslation("customerValidationSuccessMessage");
        public string ContinueToPaymentText => GetTranslation("continueToPayment");
        public string BackToCustomerInfoText => GetTranslation("backToCustomerInfo");
        public string CustomerLabelText => GetTranslation("customerTitle");
        public string MoMoInstruction => GetTranslation("momoInstruction");
        public string CardInstruction => GetTranslation("cardInstruction");
        public string BackToMethodsLabel => GetTranslation("backToMethods");
        public string PurchaseCodePlaceholder => GetTranslation("purchaseCodePlaceholder");
        public string ChangeMethodLabel => GetTranslation("changeMethod");
        public string ProcessButtonText => GetProcessingButtonText();

        [ObservableProperty]
        private decimal _subtotal;

        [ObservableProperty]
        private decimal _tax;

        [ObservableProperty]
        private decimal _grandTotal;

        [ObservableProperty]
        private bool _isCustomerInfoValid;

        [ObservableProperty]
        private string _customerValidationMessage = string.Empty;

        [ObservableProperty]
        private bool _showTinPurchaseCode;

        [ObservableProperty]
        private bool _isPhoneTabSelected = true;

        [ObservableProperty]
        private bool _isTinTabSelected = false;

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

        public string CustomerSummary
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Customer))
                    return $"Phone: {Customer}";
                else if (!string.IsNullOrWhiteSpace(TinNumber))
                    return $"TIN: {TinNumber}";
                else
                    return "No customer information";
            }
        }

        // Computed properties
        public decimal Change
        {
            get
            {
                if (SelectedMethod == "Cash" && decimal.TryParse(CashReceived.Replace(",", "").Replace(".", ""),
                    NumberStyles.Number, CultureInfo.InvariantCulture, out decimal received) && received >= Total)
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
                    return decimal.TryParse(CashReceived.Replace(",", "").Replace(".", ""),
                        NumberStyles.Number, CultureInfo.InvariantCulture, out decimal received) && received >= Total;
                }
                // For MoMo and Card, we just need a method selected and customer info valid
                return !string.IsNullOrEmpty(SelectedMethod) && IsCustomerInfoValid;
            }
        }

        public PaymentModalViewModel(ICommand closeCommand, ICommand paymentCompleteCommand)
        {
            CloseCommand = closeCommand;
            PaymentCompleteCommand = paymentCompleteCommand;

            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;

            // Initialize transaction ID
            _transactionId = _stateService.GenerateTransactionId();

            // Recalculate totals
            RecalculateTotals();

            // Initialize cart items
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

            // Subscribe to property changes for validation
            PropertyChanged += OnViewModelPropertyChanged;
        }

        partial void OnCustomerChanged(string value)
        {
            ValidateCustomerInfo();
        }

        partial void OnTinNumberChanged(string value)
        {
            ShowTinPurchaseCode = !string.IsNullOrWhiteSpace(value);
            ValidateCustomerInfo();
        }

        partial void OnPurchaseCodeChanged(string value)
        {
            ValidateCustomerInfo();
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CashReceived) || e.PropertyName == nameof(SelectedMethod))
            {
                OnPropertyChanged(nameof(Change));
                OnPropertyChanged(nameof(CanCompletePayment));
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

        [RelayCommand]
        private void ClearCustomerInfo()
        {
            Customer = string.Empty;
            TinNumber = string.Empty;
            PurchaseCode = string.Empty;
            ShowTinPurchaseCode = false;
        }

        [RelayCommand]
        private void SelectPhoneTab()
        {
            IsPhoneTabSelected = true;
            IsTinTabSelected = false;
        }

        [RelayCommand]
        private void SelectTinTab()
        {
            IsPhoneTabSelected = false;
            IsTinTabSelected = true;
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
            ShowPaymentMethods = true; // Hide payment method selection

            // Auto-fill cash received with total for cash payments
            if (method == "Cash")
            {
                CashReceived = Total.ToString("N0", CultureInfo.InvariantCulture);
            }
            else
            {
                CashReceived = string.Empty;
            }

            OnPropertyChanged(nameof(IsMethodSelected));
            OnPropertyChanged(nameof(CanCompletePayment));
            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(ProcessButtonText));
            OnPropertyChanged(nameof(SelectedMethodDisplay));
            OnPropertyChanged(nameof(SelectedMethodIcon));
            OnPropertyChanged(nameof(SelectedMethodColor));
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

        private bool ValidateCustomerInfo()
        {
            // If both are empty
            if (string.IsNullOrWhiteSpace(Customer) &&
                string.IsNullOrWhiteSpace(TinNumber))
            {
                CustomerValidationMessage = "Please provide either phone number or TIN";
                IsCustomerInfoValid = false;
                OnPropertyChanged(nameof(CanCompletePayment));
                OnPropertyChanged(nameof(CustomerSummary));
                return false;
            }

            // If TIN is provided but purchase code is empty
            if (!string.IsNullOrWhiteSpace(TinNumber) &&
                string.IsNullOrWhiteSpace(PurchaseCode))
            {
                CustomerValidationMessage = "Purchase code is required for TIN";
                IsCustomerInfoValid = false;
                OnPropertyChanged(nameof(CanCompletePayment));
                OnPropertyChanged(nameof(CustomerSummary));
                return false;
            }

            // Validate phone format (Rwandan format)
            if (!string.IsNullOrWhiteSpace(Customer))
            {
                // Remove any spaces or special characters
                var cleanPhone = Customer.Replace(" ", "").Replace("+", "").Replace("-", "");

                // Check if it's a valid Rwandan phone number (10 digits starting with 250)
                if (!long.TryParse(cleanPhone, out long phoneNumber) ||
                    (cleanPhone.Length != 12 && cleanPhone.Length != 10) ||
                    !cleanPhone.StartsWith("250"))
                {
                    CustomerValidationMessage = "Please enter a valid Rwandan phone number (e.g., +25078XXXXXXX)";
                    IsCustomerInfoValid = false;
                    OnPropertyChanged(nameof(CanCompletePayment));
                    OnPropertyChanged(nameof(CustomerSummary));
                    return false;
                }
            }

            // If TIN is provided, validate it (should be 9 digits for Rwanda)
            if (!string.IsNullOrWhiteSpace(TinNumber))
            {
                var cleanTin = TinNumber.Replace(" ", "").Replace("-", "");
                if (!long.TryParse(cleanTin, out long tin) || cleanTin.Length != 9)
                {
                    CustomerValidationMessage = "TIN should be 9 digits";
                    IsCustomerInfoValid = false;
                    OnPropertyChanged(nameof(CanCompletePayment));
                    OnPropertyChanged(nameof(CustomerSummary));
                    return false;
                }

                // Purchase code should not be empty
                if (string.IsNullOrWhiteSpace(PurchaseCode))
                {
                    CustomerValidationMessage = "Purchase code is required for TIN";
                    IsCustomerInfoValid = false;
                    OnPropertyChanged(nameof(CanCompletePayment));
                    OnPropertyChanged(nameof(CustomerSummary));
                    return false;
                }
            }

            CustomerValidationMessage = string.Empty;
            IsCustomerInfoValid = true;
            OnPropertyChanged(nameof(CanCompletePayment));
            return true;
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            if (!ValidateCustomerInfo())
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Validation Error",
                    CustomerValidationMessage,
                    "OK");
                return;
            }

            if (!CanCompletePayment) return;

            if (SelectedMethod == "Cash")
            {
                var cashReceived = decimal.Parse(CashReceived.Replace(",", "").Replace(".", ""), CultureInfo.InvariantCulture);
                var change = cashReceived - Total;

                var confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Cash Payment",
                    $"Amount Due: {Total:N0} RWF\n" +
                    $"Cash Received: {cashReceived:N0}\n" +
                    $"Change Due: {change:N0} RWF\n\n" +
                    "Do you want to proceed?",
                    "Yes, Process",
                    "Cancel");

                if (!confirm) return;
            }
            else if (SelectedMethod == "MoMo" || SelectedMethod == "Card")
            {
                var confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Payment",
                    $"Amount: {Total:N0} RWF\n" +
                    $"Method: {SelectedMethod}\n\n" +
                    $"This is a simulation. Real {SelectedMethod} payment requires external device integration.\n\n" +
                    "Continue with simulation?",
                    "Yes, Simulate",
                    "Cancel");

                if (!confirm) return;
            }

            Processing = true;

            try
            {
                // Generate new transaction ID
                var newTransactionId = TransactionId;
                TransactionId = newTransactionId;

                // Get customer identifier
                var customerIdentifier = !string.IsNullOrWhiteSpace(Customer) ? Customer : $"TIN: {TinNumber}";

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
                    Customer = customerIdentifier,
                    Subtotal = Subtotal,
                    Tax = Tax,
                    GrandTotal = GrandTotal,
                    Amount = Total,
                    Change = Change,
                    TransactionId = newTransactionId,
                    Items = new List<CartItem>(CartItems)
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
                    $"Customer: {customerIdentifier}\n" +
                    $"Payment Method: {SelectedMethod}\n" +
                    $"{(SelectedMethod == "Cash" ? $"Cash Received: {CashReceived}\nChange: {Change:N0} RWF" : "")}\n\n",
                    "OK");

                // Close modal after successful payment
                Reset();
                ClearCart();
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
            Tax = Subtotal * 0.1m; // 10% tax
            GrandTotal = Subtotal + Tax;
            Total = GrandTotal;
        }


        private void ClearCart()
        {
            // Use the public method to clear cart
            _stateService.ClearCart();

            // Update the local cart items
            CartItems.Clear();

            // Recalculate totals
            RecalculateTotals();

            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(GrandTotal));
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
            ShowPaymentMethods = true;

            OnPropertyChanged(nameof(IsMethodSelected));
            OnPropertyChanged(nameof(CanCompletePayment));
            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(ProcessButtonText));
            OnPropertyChanged(nameof(SelectedMethodDisplay));
            OnPropertyChanged(nameof(SelectedMethodIcon));
            OnPropertyChanged(nameof(SelectedMethodColor));
        }

        [RelayCommand]
        private void ClosePaymentModal()
        {
            // Reset the modal state
            Reset();

            // Update ApplicationStateService to close modal
            _stateService.ShowPaymentModal = false;

            // Also execute the parent close command if provided
            if (CloseCommand?.CanExecute(null) == true)
            {
                CloseCommand.Execute(null);
            }
        }

        public void Reset()
        {
            SelectedMethod = null;
            CashReceived = string.Empty;
            Customer = string.Empty;
            TinNumber = string.Empty;
            PurchaseCode = string.Empty;
            ShowTinPurchaseCode = false;
            ShowPaymentMethods = false;

            Processing = false;
            TransactionId = _stateService.GenerateTransactionId();

            // Recalculate totals
            RecalculateTotals();

            // Reset validation
            IsCustomerInfoValid = false;
            CustomerValidationMessage = string.Empty;

            OnPropertyChanged(nameof(IsMethodSelected));
            OnPropertyChanged(nameof(CanCompletePayment));
            OnPropertyChanged(nameof(Change));
            OnPropertyChanged(nameof(ProcessButtonText));
            OnPropertyChanged(nameof(TransactionId));
            OnPropertyChanged(nameof(SelectedMethodDisplay));
            OnPropertyChanged(nameof(SelectedMethodIcon));
            OnPropertyChanged(nameof(SelectedMethodColor));
            OnPropertyChanged(nameof(CustomerSummary));
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

                        // Recalculate totals
                        RecalculateTotals();

                        OnPropertyChanged(nameof(CartItems));
                        OnPropertyChanged(nameof(Total));
                        OnPropertyChanged(nameof(Subtotal));
                        OnPropertyChanged(nameof(Tax));
                        OnPropertyChanged(nameof(GrandTotal));
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

        public string SelectedMethodDisplay => SelectedMethod switch
        {
            "Cash" => CashLabel,
            "MoMo" => MoMoLabel,
            "Card" => CardLabel,
            _ => "Select Method"
        };

        public string SelectedMethodIcon => SelectedMethod switch
        {
            "Cash" => "💵",
            "MoMo" => "📱",
            "Card" => "💳",
            _ => "❓"
        };

        public Color SelectedMethodColor => SelectedMethod switch
        {
            "Cash" => Color.FromArgb("#10B981"),
            "MoMo" => Color.FromArgb("#3B82F6"),
            "Card" => Color.FromArgb("#F59E0B"),
            _ => Colors.Gray
        };

        [RelayCommand]
        private void ContinueToPayment()
        {
            ShowPaymentMethods = true;
        }

        [RelayCommand]
        private void GoBackToCustomerInfo()
        {
            ShowPaymentMethods = false;
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
            OnPropertyChanged(nameof(CustomerInfoTitle));
            OnPropertyChanged(nameof(OrLabel));
            OnPropertyChanged(nameof(TinLabel));
            OnPropertyChanged(nameof(ClearTinButtonText));
            OnPropertyChanged(nameof(PurchaseCodeLabel));
            OnPropertyChanged(nameof(PurchaseCodeAlert));
            OnPropertyChanged(nameof(CustomerValidationSuccessMessage));
            OnPropertyChanged(nameof(ContinueToPaymentText));
            OnPropertyChanged(nameof(BackToCustomerInfoText));
            OnPropertyChanged(nameof(CustomerLabelText));
            OnPropertyChanged(nameof(MoMoInstruction));
            OnPropertyChanged(nameof(CardInstruction));
            OnPropertyChanged(nameof(BackToMethodsLabel));
            OnPropertyChanged(nameof(PurchaseCodePlaceholder));
            OnPropertyChanged(nameof(ChangeMethodLabel));
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
                    [Language.EN] = "Phone Number",
                    [Language.RW] = "Nimero ya Telefoni",
                    [Language.FR] = "Numéro de Téléphone"
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
                    [Language.RW] = "IKARITA",
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
                },
                ["customerInfoTitle"] = new()
                {
                    [Language.EN] = "Customer Information *",
                    [Language.RW] = "Amakuru y'Umukiriya *",
                    [Language.FR] = "Informations Client *"
                },
                ["or"] = new()
                {
                    [Language.EN] = "OR",
                    [Language.RW] = "CYANGWA",
                    [Language.FR] = "OU"
                },
                ["tin"] = new()
                {
                    [Language.EN] = "TIN Number (Tax Identification)",
                    [Language.RW] = "Nimero ya TIN (Indangamuntu y'Imisoro)",
                    [Language.FR] = "Numéro TIN (Identification Fiscale)"
                },
                ["clear"] = new()
                {
                    [Language.EN] = "Clear",
                    [Language.RW] = "Siba",
                    [Language.FR] = "Effacer"
                },
                ["purchaseCode"] = new()
                {
                    [Language.EN] = "Purchase Code *",
                    [Language.RW] = "Kode y'Igicuruzwa *",
                    [Language.FR] = "Code d'Achat *"
                },
                ["purchaseCodeAlert"] = new()
                {
                    [Language.EN] = "Purchase is required when using TIN",
                    [Language.RW] = "Kode y'Igicuruzwa irakenewe iyo ukoresha TIN",
                    [Language.FR] = "Le code d'achat est requis lors de l'utilisation du TIN"
                },
                ["customerValidationSuccessMessage"] = new()
                {
                    [Language.EN] = "Customer information verified. Ready to proceed to payment.",
                    [Language.RW] = "Amakuru y'umukiriya yemejwe. Biteguye gukomeza kwishyura.",
                    [Language.FR] = "Informations client vérifiées. Prêt à procéder au paiement."
                },
                ["continueToPayment"] = new()
                {
                    [Language.EN] = "Continue to Payment →",
                    [Language.RW] = "Komeza Kwishyura →",  
                    [Language.FR] = "Continuer vers le Paiement →"
                },
                ["backToCustomerInfo"] = new()
                {
                    [Language.EN] = "← Edit Customer Information",
                    [Language.RW] = "← Hindura Amakuru y'Umukiriya",
                    [Language.FR] = "← Modifier les Informations Client"
                },
                ["customerTitle"] = new()
                {
                    [Language.EN] = "Customer",
                    [Language.RW] = "Umukiriya",
                    [Language.FR] = "Client"
                },
                ["momoInstruction"] = new()
                {
                    [Language.EN] = "This feature requires external payment device integration.",
                    [Language.RW] = "Iyi gahunda isaba kwinjiza igikoresho cyo kwishyura hanze.",
                    [Language.FR] = "Cette fonctionnalité nécessite une intégration de dispositif de paiement externe."
                },
                ["cardInstruction"] = new()
                {
                    [Language.EN] = "This feature requires external card reader integration.",
                    [Language.RW] = "Iyi gahunda isaba kwinjiza icyuma gisesengura amakarita hanze.",
                    [Language.FR] = "Cette fonctionnalité nécessite une intégration de lecteur de carte externe."
                },
                ["backToMethods"] = new()
                {
                    [Language.EN] = "Back to Methods",
                    [Language.RW] = "Garuka ku buryo bwo Kwishyura",
                    [Language.FR] = "Retour aux Méthodes"
                },
                ["purchaseCodePlaceholder"] = new()
                {
                    [Language.EN] = "Enter Purchase Code",
                    [Language.RW] = "Andika Kode y'Igicuruzwa",
                    [Language.FR] = "Entrez le Code d'Achat"
                },
                ["changeMethod"] = new()
                {
                    [Language.EN] = "Change Payment Method",
                    [Language.RW] = "Hindura Uburyo bwo Kwishyura",
                    [Language.FR] = "Changer le Mode de Paiement"
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
            PropertyChanged -= OnViewModelPropertyChanged;
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