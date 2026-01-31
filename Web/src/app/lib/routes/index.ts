import { Router } from "express";
import { ServiceCategoriesController } from "../controllers/serviceCategoriesController";
import { ServicesController } from "../controllers/servicesController";
import { TransactionsController } from "../controllers/transactionsController";
import { PaymentRecordsController } from "../controllers/paymentRecordsController";

const router = Router();

// Initialize controllers
const serviceCategoriesController = new ServiceCategoriesController();
const servicesController = new ServicesController();
const transactionsController = new TransactionsController();
const paymentRecordsController = new PaymentRecordsController();

// Service Categories Routes
router.get(
  "/service-categories",
  serviceCategoriesController.getAll.bind(serviceCategoriesController),
);
router.get(
  "/service-categories/active",
  serviceCategoriesController.getActive.bind(serviceCategoriesController),
);
router.get(
  "/service-categories/:id",
  serviceCategoriesController.getById.bind(serviceCategoriesController),
);
router.get(
  "/service-categories/type/:type",
  serviceCategoriesController.getByType.bind(serviceCategoriesController),
);
router.post(
  "/service-categories",
  serviceCategoriesController.create.bind(serviceCategoriesController),
);
router.put(
  "/service-categories/:id",
  serviceCategoriesController.update.bind(serviceCategoriesController),
);
router.delete(
  "/service-categories/:id",
  serviceCategoriesController.delete.bind(serviceCategoriesController),
);

// Services Routes
router.get("/services", servicesController.getAll.bind(servicesController));
router.get(
  "/services/available",
  servicesController.getAvailable.bind(servicesController),
);
router.get(
  "/services/:id",
  servicesController.getById.bind(servicesController),
);
router.get(
  "/services/category/:categoryId",
  servicesController.getByCategory.bind(servicesController),
);
router.post("/services", servicesController.create.bind(servicesController));
router.put("/services/:id", servicesController.update.bind(servicesController));
router.delete(
  "/services/:id",
  servicesController.delete.bind(servicesController),
);

// Transactions Routes
router.get(
  "/transactions",
  transactionsController.getAll.bind(transactionsController),
);
router.get(
  "/transactions/date-range",
  transactionsController.getByDateRange.bind(transactionsController),
);
router.get(
  "/transactions/status/:status",
  transactionsController.getByStatus.bind(transactionsController),
);
router.get(
  "/transactions/:id",
  transactionsController.getById.bind(transactionsController),
);
router.get(
  "/transactions/transaction-id/:transactionId",
  transactionsController.getByTransactionId.bind(transactionsController),
);
router.post(
  "/transactions",
  transactionsController.create.bind(transactionsController),
);
router.put(
  "/transactions/:id",
  transactionsController.update.bind(transactionsController),
);
router.delete(
  "/transactions/:id",
  transactionsController.delete.bind(transactionsController),
);

// Payment Records Routes
router.get(
  "/payment-records",
  paymentRecordsController.getAll.bind(paymentRecordsController),
);
router.get(
  "/payment-records/date-range",
  paymentRecordsController.getByDateRange.bind(paymentRecordsController),
);
router.get(
  "/payment-records/status/:status",
  paymentRecordsController.getByStatus.bind(paymentRecordsController),
);
router.get(
  "/payment-records/:id",
  paymentRecordsController.getById.bind(paymentRecordsController),
);
router.get(
  "/payment-records/transaction/:transactionId",
  paymentRecordsController.getByTransactionId.bind(paymentRecordsController),
);
router.post(
  "/payment-records",
  paymentRecordsController.create.bind(paymentRecordsController),
);
router.put(
  "/payment-records/:id",
  paymentRecordsController.update.bind(paymentRecordsController),
);
router.delete(
  "/payment-records/:id",
  paymentRecordsController.delete.bind(paymentRecordsController),
);

export default router;
