// قاعدة بيانات تطبيق المحاسبة
const DB_NAME = 'AccountingAppDB';
const DB_VERSION = 3; // زيادة إصدار قاعدة البيانات لإصلاح المشاكل

// كائن قاعدة البيانات
let db;

// حذف قاعدة البيانات القديمة
function deleteDatabase() {
    return new Promise((resolve, reject) => {
        console.log('جاري حذف قاعدة البيانات القديمة...');
        const deleteRequest = indexedDB.deleteDatabase(DB_NAME);

        deleteRequest.onsuccess = function() {
            console.log('تم حذف قاعدة البيانات القديمة بنجاح');
            resolve(true);
        };

        deleteRequest.onerror = function(event) {
            console.error('خطأ في حذف قاعدة البيانات القديمة:', event.target.error);
            reject(event.target.error);
        };
    });
}

// تهيئة قاعدة البيانات
function initDatabase() {
    return new Promise((resolve, reject) => {
        // التحقق من وجود قاعدة البيانات القديمة
        const checkRequest = indexedDB.open(DB_NAME);

        checkRequest.onsuccess = function(event) {
            const oldDb = event.target.result;
            const oldVersion = oldDb.version;
            oldDb.close();

            // إذا كان الإصدار القديم أقل من الإصدار الحالي، قم بحذف قاعدة البيانات القديمة
            if (oldVersion < DB_VERSION) {
                deleteDatabase()
                    .then(() => openDatabase())
                    .then(db => resolve(db))
                    .catch(error => reject(error));
            } else {
                openDatabase()
                    .then(db => resolve(db))
                    .catch(error => reject(error));
            }
        };

        checkRequest.onerror = function(event) {
            console.error('خطأ في فحص قاعدة البيانات:', event.target.error);
            openDatabase()
                .then(db => resolve(db))
                .catch(error => reject(error));
        };
    });
}

// فتح قاعدة البيانات
function openDatabase() {
    return new Promise((resolve, reject) => {
        console.log('جاري فتح قاعدة البيانات...');
        const request = indexedDB.open(DB_NAME, DB_VERSION);

        // إضافة معالج الأخطاء
        request.onerror = function(event) {
            console.error('خطأ في فتح قاعدة البيانات:', event.target.error);
            reject(event.target.error);
        };

        // إضافة معالج النجاح
        request.onsuccess = function(event) {
            db = event.target.result;
            console.log('تم فتح قاعدة البيانات بنجاح');

            // طباعة أسماء مخازن البيانات المتاحة
            console.log('مخازن البيانات المتاحة:', Array.from(db.objectStoreNames));

            resolve(db);
        };

        // إنشاء مخازن البيانات عند إنشاء قاعدة البيانات لأول مرة
        request.onupgradeneeded = function(event) {
            db = event.target.result;
            const oldVersion = event.oldVersion;

            // إنشاء المخازن الأساسية إذا كانت قاعدة البيانات جديدة
            if (oldVersion < 1) {
                console.log('إنشاء مخازن البيانات الأساسية');

                // إنشاء مخزن المنتجات
                if (!db.objectStoreNames.contains('products')) {
                    const productsStore = db.createObjectStore('products', { keyPath: 'id', autoIncrement: true });
                    productsStore.createIndex('code', 'code', { unique: true });
                    productsStore.createIndex('barcode', 'barcode', { unique: false });
                    productsStore.createIndex('name', 'name', { unique: false });
                }

                                // إنشاء مخزن العملاء
                if (!db.objectStoreNames.contains('customers')) {
                    const customersStore = db.createObjectStore('customers', { keyPath: 'id', autoIncrement: true });
                    customersStore.createIndex('code', 'code', { unique: true });
                    customersStore.createIndex('name', 'name', { unique: false });
                    customersStore.createIndex('phone', 'phone', { unique: false });
                }

                // إنشاء مخزن الموردين
                if (!db.objectStoreNames.contains('suppliers')) {
                    const suppliersStore = db.createObjectStore('suppliers', { keyPath: 'id', autoIncrement: true });
                    suppliersStore.createIndex('code', 'code', { unique: true });
                    suppliersStore.createIndex('name', 'name', { unique: false });
                    suppliersStore.createIndex('phone', 'phone', { unique: false });
                }

                // إنشاء مخزن المصروفات
                if (!db.objectStoreNames.contains('expenses')) {
                    const expensesStore = db.createObjectStore('expenses', { keyPath: 'id', autoIncrement: true });
                    expensesStore.createIndex('date', 'date', { unique: false });
                    expensesStore.createIndex('category', 'category', { unique: false });
                    expensesStore.createIndex('paymentMethod', 'paymentMethod', { unique: false });
                }

                // إنشاء مخزن الفواتير
                if (!db.objectStoreNames.contains('invoices')) {
                    const invoicesStore = db.createObjectStore('invoices', { keyPath: 'id', autoIncrement: true });
                    invoicesStore.createIndex('invoiceNumber', 'invoiceNumber', { unique: true });
                    invoicesStore.createIndex('customerId', 'customerId', { unique: false });
                    invoicesStore.createIndex('date', 'date', { unique: false });
                    invoicesStore.createIndex('type', 'type', { unique: false });
                }

                // إنشاء مخزن بنود الفواتير
                if (!db.objectStoreNames.contains('invoiceItems')) {
                    const invoiceItemsStore = db.createObjectStore('invoiceItems', { keyPath: 'id', autoIncrement: true });
                    invoiceItemsStore.createIndex('invoiceId', 'invoiceId', { unique: false });
                    invoiceItemsStore.createIndex('productId', 'productId', { unique: false });
                }

                // إنشاء مخزن المستخدمين
                if (!db.objectStoreNames.contains('users')) {
                    const usersStore = db.createObjectStore('users', { keyPath: 'id', autoIncrement: true });
                    usersStore.createIndex('username', 'username', { unique: true });

                    // إضافة مستخدم افتراضي
                    const defaultUser = {
                        username: 'admin',
                        password: 'admin123', // في التطبيق الحقيقي يجب تشفير كلمة المرور
                        fullName: 'مدير النظام',
                        role: 'admin',
                        isActive: true,
                        createdAt: new Date()
                    };

                    usersStore.add(defaultUser);
                }
            }

            // إضافة المخازن الجديدة في الإصدار 2
            if (oldVersion < 2) {
                console.log('إضافة مخازن البيانات الجديدة للإصدار 2');

                // إنشاء مخزن حركات المخزون
                if (!db.objectStoreNames.contains('inventory')) {
                    const inventoryStore = db.createObjectStore('inventory', { keyPath: 'id', autoIncrement: true });
                    inventoryStore.createIndex('productId', 'productId', { unique: false });
                    inventoryStore.createIndex('date', 'date', { unique: false });
                    inventoryStore.createIndex('type', 'type', { unique: false });
                    inventoryStore.createIndex('referenceId', 'referenceId', { unique: false });
                }

                // إنشاء مخزن إعدادات النظام
                if (!db.objectStoreNames.contains('settings')) {
                    const settingsStore = db.createObjectStore('settings', { keyPath: 'id', autoIncrement: true });
                    settingsStore.createIndex('key', 'key', { unique: true });

                    // إضافة إعدادات افتراضية
                    settingsStore.add({
                        key: 'companyInfo',
                        value: {
                            name: 'شركة أبو سليمان للتجارة',
                            address: 'الرياض - المملكة العربية السعودية',
                            phone: '0555123456',
                            email: 'info@abosuliman.com',
                            taxNumber: '123456789',
                            logo: ''
                        }
                    });

                    settingsStore.add({
                        key: 'invoiceSettings',
                        value: {
                            taxRate: 15,
                            invoicePrefix: 'INV-',
                            termsAndConditions: 'شروط وأحكام الفاتورة'
                        }
                    });
                }
            }

            // إضافة المخازن الجديدة في الإصدار 3
            if (oldVersion < 3) {
                console.log('إضافة مخازن البيانات الجديدة للإصدار 3');

                // التأكد من وجود مخزن الموردين
                if (!db.objectStoreNames.contains('suppliers')) {
                    console.log('إنشاء مخزن الموردين');
                    const suppliersStore = db.createObjectStore('suppliers', { keyPath: 'id', autoIncrement: true });
                    suppliersStore.createIndex('code', 'code', { unique: true });
                    suppliersStore.createIndex('name', 'name', { unique: false });
                    suppliersStore.createIndex('phone', 'phone', { unique: false });

                    // إضافة مورد افتراضي
                    const transaction = db.transaction(['suppliers'], 'readwrite');
                    const store = transaction.objectStore('suppliers');
                    store.add({
                        code: 'SUP-001',
                        name: 'مورد افتراضي',
                        contactPerson: 'محمد',
                        phone: '0555123456',
                        email: 'supplier@example.com',
                        address: 'الرياض',
                        balance: 0,
                        isActive: true,
                        createdAt: new Date()
                    });
                }

                // التأكد من وجود مخزن الفواتير
                if (!db.objectStoreNames.contains('invoices')) {
                    console.log('إنشاء مخزن الفواتير');
                    const invoicesStore = db.createObjectStore('invoices', { keyPath: 'id', autoIncrement: true });
                    invoicesStore.createIndex('invoiceNumber', 'invoiceNumber', { unique: true });
                    invoicesStore.createIndex('customerId', 'customerId', { unique: false });
                    invoicesStore.createIndex('date', 'date', { unique: false });
                    invoicesStore.createIndex('type', 'type', { unique: false });
                }
            }
        };

        // لا نحتاج إلى إضافة معالجي النجاح والفشل هنا لأننا أضفناهما في بداية الوظيفة
    });
}

// ======== وظائف إدارة المنتجات ========

// إضافة منتج جديد
function addProduct(product) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['products'], 'readwrite');
        const store = transaction.objectStore('products');

        // التحقق من عدم وجود منتج بنفس الكود
        const codeIndex = store.index('code');
        const codeRequest = codeIndex.get(product.code);

        codeRequest.onsuccess = function(event) {
            if (event.target.result) {
                reject(new Error('يوجد منتج بنفس الكود'));
                return;
            }

            // إضافة المنتج
            const request = store.add(product);

            request.onsuccess = function(event) {
                resolve(event.target.result); // إرجاع معرف المنتج الجديد
            };

            request.onerror = function(event) {
                reject(event.target.error);
            };
        };

        codeRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على جميع المنتجات
function getAllProducts() {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['products'], 'readonly');
        const store = transaction.objectStore('products');
        const request = store.getAll();

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على منتج بواسطة المعرف
function getProductById(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['products'], 'readonly');
        const store = transaction.objectStore('products');
        const request = store.get(id);

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// تحديث منتج
function updateProduct(product) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['products'], 'readwrite');
        const store = transaction.objectStore('products');

        // التحقق من عدم وجود منتج آخر بنفس الكود
        const codeIndex = store.index('code');
        const codeRequest = codeIndex.get(product.code);

        codeRequest.onsuccess = function(event) {
            const existingProduct = event.target.result;
            if (existingProduct && existingProduct.id !== product.id) {
                reject(new Error('يوجد منتج آخر بنفس الكود'));
                return;
            }

            // تحديث المنتج
            const request = store.put(product);

            request.onsuccess = function(event) {
                resolve(event.target.result);
            };

            request.onerror = function(event) {
                reject(event.target.error);
            };
        };

        codeRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// حذف منتج
function deleteProduct(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['products'], 'readwrite');
        const store = transaction.objectStore('products');
        const request = store.delete(id);

        request.onsuccess = function(event) {
            resolve(true);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// ======== وظائف إدارة العملاء ========

// إضافة عميل جديد
function addCustomer(customer) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['customers'], 'readwrite');
        const store = transaction.objectStore('customers');

        // التحقق من عدم وجود عميل بنفس الكود
        const codeIndex = store.index('code');
        const codeRequest = codeIndex.get(customer.code);

        codeRequest.onsuccess = function(event) {
            if (event.target.result) {
                reject(new Error('يوجد عميل بنفس الكود'));
                return;
            }

            // إضافة العميل
            const request = store.add(customer);

            request.onsuccess = function(event) {
                resolve(event.target.result); // إرجاع معرف العميل الجديد
            };

            request.onerror = function(event) {
                reject(event.target.error);
            };
        };

        codeRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على جميع العملاء
function getAllCustomers() {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['customers'], 'readonly');
        const store = transaction.objectStore('customers');
        const request = store.getAll();

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على عميل بواسطة المعرف
function getCustomerById(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['customers'], 'readonly');
        const store = transaction.objectStore('customers');
        const request = store.get(id);

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// تحديث عميل
function updateCustomer(customer) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['customers'], 'readwrite');
        const store = transaction.objectStore('customers');

        // التحقق من عدم وجود عميل آخر بنفس الكود
        const codeIndex = store.index('code');
        const codeRequest = codeIndex.get(customer.code);

        codeRequest.onsuccess = function(event) {
            const existingCustomer = event.target.result;
            if (existingCustomer && existingCustomer.id !== customer.id) {
                reject(new Error('يوجد عميل آخر بنفس الكود'));
                return;
            }

            // تحديث العميل
            const request = store.put(customer);

            request.onsuccess = function(event) {
                resolve(event.target.result);
            };

            request.onerror = function(event) {
                reject(event.target.error);
            };
        };

        codeRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// حذف عميل
function deleteCustomer(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['customers'], 'readwrite');
        const store = transaction.objectStore('customers');
        const request = store.delete(id);

        request.onsuccess = function(event) {
            resolve(true);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// ======== وظائف إدارة الفواتير ========

// إضافة فاتورة جديدة مع بنودها
function addInvoice(invoice, items) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['invoices', 'invoiceItems'], 'readwrite');
        const invoicesStore = transaction.objectStore('invoices');
        const invoiceItemsStore = transaction.objectStore('invoiceItems');

        // التحقق من عدم وجود فاتورة بنفس الرقم
        const invoiceNumberIndex = invoicesStore.index('invoiceNumber');
        const invoiceNumberRequest = invoiceNumberIndex.get(invoice.invoiceNumber);

        invoiceNumberRequest.onsuccess = function(event) {
            if (event.target.result) {
                reject(new Error('يوجد فاتورة بنفس الرقم'));
                return;
            }

            // إضافة الفاتورة
            const invoiceRequest = invoicesStore.add(invoice);

            invoiceRequest.onsuccess = function(event) {
                const invoiceId = event.target.result;

                // إضافة بنود الفاتورة
                let itemsAdded = 0;

                items.forEach(item => {
                    item.invoiceId = invoiceId;
                    const itemRequest = invoiceItemsStore.add(item);

                    itemRequest.onsuccess = function() {
                        itemsAdded++;
                        if (itemsAdded === items.length) {
                            resolve(invoiceId);
                        }
                    };

                    itemRequest.onerror = function(event) {
                        reject(event.target.error);
                    };
                });
            };

            invoiceRequest.onerror = function(event) {
                reject(event.target.error);
            };
        };

        invoiceNumberRequest.onerror = function(event) {
            reject(event.target.error);
        };

        // إدارة الأخطاء في المعاملة
        transaction.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على جميع الفواتير
function getAllInvoices() {
    return new Promise((resolve, reject) => {
        try {
            // التحقق من وجود قاعدة البيانات
            if (!db) {
                console.error('خطأ: قاعدة البيانات غير متاحة');
                return reject(new Error('قاعدة البيانات غير متاحة'));
            }

            // التحقق من وجود مخازن البيانات
            if (!db.objectStoreNames.contains('invoices') || !db.objectStoreNames.contains('customers')) {
                console.error('خطأ: مخازن البيانات "invoices" أو "customers" غير موجودة');

                // إعادة تهيئة قاعدة البيانات
                return deleteDatabase()
                    .then(() => initDatabase())
                    .then(() => {
                        // إرجاع مصفوفة فارغة لأن قاعدة البيانات تم إعادة تهيئتها
                        resolve([]);
                    })
                    .catch(error => {
                        reject(error);
                    });
            }

            const transaction = db.transaction(['invoices', 'customers'], 'readonly');
            const invoicesStore = transaction.objectStore('invoices');
            const customersStore = transaction.objectStore('customers');
            const request = invoicesStore.getAll();

            request.onsuccess = function(event) {
                const invoices = event.target.result || [];
                console.log(`تم استرجاع ${invoices.length} فاتورة من قاعدة البيانات`);

                // إضافة معلومات العميل لكل فاتورة
                let invoicesProcessed = 0;

                if (invoices.length === 0) {
                    resolve([]);
                    return;
                }

                invoices.forEach((invoice, index) => {
                    try {
                        const customerRequest = customersStore.get(invoice.customerId);

                        customerRequest.onsuccess = function(event) {
                            const customer = event.target.result;
                            invoices[index].customerName = customer ? customer.name : 'غير معروف';

                            invoicesProcessed++;
                            if (invoicesProcessed === invoices.length) {
                                resolve(invoices);
                            }
                        };

                        customerRequest.onerror = function(event) {
                            console.error('خطأ في استرجاع العميل:', event.target.error);
                            invoices[index].customerName = 'غير معروف';

                            invoicesProcessed++;
                            if (invoicesProcessed === invoices.length) {
                                resolve(invoices);
                            }
                        };
                    } catch (error) {
                        console.error('خطأ في معالجة الفاتورة:', error);
                        invoices[index].customerName = 'غير معروف';

                        invoicesProcessed++;
                        if (invoicesProcessed === invoices.length) {
                            resolve(invoices);
                        }
                    }
                });
            };

            request.onerror = function(event) {
                console.error('خطأ في استرجاع الفواتير:', event.target.error);
                reject(event.target.error);
            };
        } catch (error) {
            console.error('خطأ غير متوقع في استرجاع الفواتير:', error);
            reject(error);
        }
    });
}

// الحصول على فاتورة مع بنودها
function getInvoiceWithItems(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['invoices', 'invoiceItems', 'products', 'customers'], 'readonly');
        const invoicesStore = transaction.objectStore('invoices');
        const invoiceItemsStore = transaction.objectStore('invoiceItems');
        const productsStore = transaction.objectStore('products');
        const customersStore = transaction.objectStore('customers');

        // الحصول على الفاتورة
        const invoiceRequest = invoicesStore.get(id);

        invoiceRequest.onsuccess = function(event) {
            const invoice = event.target.result;

            if (!invoice) {
                reject(new Error('الفاتورة غير موجودة'));
                return;
            }

            // الحصول على العميل
            const customerRequest = customersStore.get(invoice.customerId);

            customerRequest.onsuccess = function(event) {
                const customer = event.target.result;
                invoice.customerName = customer ? customer.name : 'غير معروف';

                // الحصول على بنود الفاتورة
                const itemsIndex = invoiceItemsStore.index('invoiceId');
                const itemsRequest = itemsIndex.getAll(id);

                itemsRequest.onsuccess = function(event) {
                    const items = event.target.result;

                    // إضافة معلومات المنتج لكل بند
                    let itemsProcessed = 0;

                    if (items.length === 0) {
                        invoice.items = [];
                        resolve(invoice);
                        return;
                    }

                    items.forEach((item, index) => {
                        const productRequest = productsStore.get(item.productId);

                        productRequest.onsuccess = function(event) {
                            const product = event.target.result;
                            items[index].productName = product ? product.name : 'غير معروف';

                            itemsProcessed++;
                            if (itemsProcessed === items.length) {
                                invoice.items = items;
                                resolve(invoice);
                            }
                        };

                        productRequest.onerror = function(event) {
                            items[index].productName = 'غير معروف';

                            itemsProcessed++;
                            if (itemsProcessed === items.length) {
                                invoice.items = items;
                                resolve(invoice);
                            }
                        };
                    });
                };

                itemsRequest.onerror = function(event) {
                    reject(event.target.error);
                };
            };

            customerRequest.onerror = function(event) {
                invoice.customerName = 'غير معروف';
                reject(event.target.error);
            };
        };

        invoiceRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// حذف فاتورة وبنودها
function deleteInvoice(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['invoices', 'invoiceItems'], 'readwrite');
        const invoicesStore = transaction.objectStore('invoices');
        const invoiceItemsStore = transaction.objectStore('invoiceItems');

        // حذف بنود الفاتورة أولاً
        const itemsIndex = invoiceItemsStore.index('invoiceId');
        const itemsRequest = itemsIndex.getAll(id);

        itemsRequest.onsuccess = function(event) {
            const items = event.target.result;

            // حذف كل بند
            items.forEach(item => {
                invoiceItemsStore.delete(item.id);
            });

            // ثم حذف الفاتورة نفسها
            const invoiceRequest = invoicesStore.delete(id);

            invoiceRequest.onsuccess = function(event) {
                resolve(true);
            };

            invoiceRequest.onerror = function(event) {
                reject(event.target.error);
            };
        };

        itemsRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// ======== وظائف المصادقة ========

// التحقق من صحة بيانات تسجيل الدخول
function login(username, password) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['users'], 'readonly');
        const store = transaction.objectStore('users');
        const index = store.index('username');
        const request = index.get(username);

        request.onsuccess = function(event) {
            const user = event.target.result;

            if (!user) {
                reject(new Error('اسم المستخدم غير موجود'));
                return;
            }

            if (user.password !== password) { // في التطبيق الحقيقي يجب التحقق من كلمة المرور المشفرة
                reject(new Error('كلمة المرور غير صحيحة'));
                return;
            }

            if (!user.isActive) {
                reject(new Error('الحساب غير نشط'));
                return;
            }

            // تحديث وقت آخر تسجيل دخول
            const updateTransaction = db.transaction(['users'], 'readwrite');
            const updateStore = updateTransaction.objectStore('users');
            user.lastLogin = new Date();
            updateStore.put(user);

            // إرجاع بيانات المستخدم (بدون كلمة المرور)
            const userInfo = { ...user };
            delete userInfo.password;
            resolve(userInfo);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// ======== وظائف إدارة المخزون ========

// إضافة حركة مخزون جديدة
function addInventoryTransaction(transaction) {
    return new Promise((resolve, reject) => {
        const tx = db.transaction(['inventory', 'products'], 'readwrite');
        const inventoryStore = tx.objectStore('inventory');
        const productsStore = tx.objectStore('products');

        // إضافة الحركة
        const request = inventoryStore.add(transaction);

        request.onsuccess = function(event) {
            const transactionId = event.target.result;

            // تحديث كمية المخزون في المنتج
            const productRequest = productsStore.get(transaction.productId);

            productRequest.onsuccess = function(event) {
                const product = event.target.result;

                if (!product) {
                    reject(new Error('المنتج غير موجود'));
                    return;
                }

                // تحديث كمية المخزون بناءً على نوع الحركة
                if (!product.stock) {
                    product.stock = 0;
                }

                switch (transaction.type) {
                    case 'add':
                        product.stock += transaction.quantity;
                        break;
                    case 'subtract':
                        product.stock -= transaction.quantity;
                        if (product.stock < 0) product.stock = 0;
                        break;
                    case 'set':
                        product.stock = transaction.quantity;
                        break;
                }

                // تحديث المنتج
                const updateRequest = productsStore.put(product);

                updateRequest.onsuccess = function() {
                    resolve(transactionId);
                };

                updateRequest.onerror = function(event) {
                    reject(event.target.error);
                };
            };

            productRequest.onerror = function(event) {
                reject(event.target.error);
            };
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على حركات المخزون لمنتج معين
function getInventoryTransactions(productId = null) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['inventory', 'products', 'users'], 'readonly');
        const inventoryStore = transaction.objectStore('inventory');
        const productsStore = transaction.objectStore('products');
        const usersStore = transaction.objectStore('users');

        let request;

        if (productId) {
            const index = inventoryStore.index('productId');
            request = index.getAll(productId);
        } else {
            request = inventoryStore.getAll();
        }

        request.onsuccess = function(event) {
            const transactions = event.target.result;

            // إضافة معلومات المنتج والمستخدم لكل حركة
            let transactionsProcessed = 0;

            if (transactions.length === 0) {
                resolve([]);
                return;
            }

            transactions.forEach((transaction, index) => {
                // إضافة معلومات المنتج
                const productRequest = productsStore.get(transaction.productId);

                productRequest.onsuccess = function(event) {
                    const product = event.target.result;
                    transactions[index].productName = product ? product.name : 'غير معروف';

                    // إضافة معلومات المستخدم
                    if (transaction.userId) {
                        const userRequest = usersStore.get(transaction.userId);

                        userRequest.onsuccess = function(event) {
                            const user = event.target.result;
                            transactions[index].userName = user ? user.fullName : 'غير معروف';

                            transactionsProcessed++;
                            if (transactionsProcessed === transactions.length) {
                                resolve(transactions);
                            }
                        };

                        userRequest.onerror = function() {
                            transactions[index].userName = 'غير معروف';

                            transactionsProcessed++;
                            if (transactionsProcessed === transactions.length) {
                                resolve(transactions);
                            }
                        };
                    } else {
                        transactions[index].userName = 'غير معروف';

                        transactionsProcessed++;
                        if (transactionsProcessed === transactions.length) {
                            resolve(transactions);
                        }
                    }
                };

                productRequest.onerror = function() {
                    transactions[index].productName = 'غير معروف';
                    transactions[index].userName = 'غير معروف';

                    transactionsProcessed++;
                    if (transactionsProcessed === transactions.length) {
                        resolve(transactions);
                    }
                };
            });
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// ======== وظائف التقارير ========

// الحصول على تقرير المبيعات
function getSalesReport(fromDate, toDate, customerId = null, status = null) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['invoices', 'invoiceItems', 'products', 'customers'], 'readonly');
        const invoicesStore = transaction.objectStore('invoices');
        const invoiceItemsStore = transaction.objectStore('invoiceItems');
        const productsStore = transaction.objectStore('products');
        const customersStore = transaction.objectStore('customers');

        const request = invoicesStore.getAll();

        request.onsuccess = function(event) {
            let invoices = event.target.result;

            // تصفية الفواتير حسب النوع (مبيعات فقط)
            invoices = invoices.filter(invoice => invoice.type === 'sale');

            // تصفية الفواتير حسب التاريخ
            if (fromDate) {
                const fromDateObj = new Date(fromDate);
                invoices = invoices.filter(invoice => new Date(invoice.date) >= fromDateObj);
            }

            if (toDate) {
                const toDateObj = new Date(toDate);
                toDateObj.setHours(23, 59, 59, 999); // نهاية اليوم
                invoices = invoices.filter(invoice => new Date(invoice.date) <= toDateObj);
            }

            // تصفية الفواتير حسب العميل
            if (customerId) {
                invoices = invoices.filter(invoice => invoice.customerId === customerId);
            }

            // تصفية الفواتير حسب الحالة
            if (status) {
                invoices = invoices.filter(invoice => invoice.status === status);
            }

            // حساب الإحصائيات
            const stats = {
                totalSales: 0,
                totalPaid: 0,
                totalDue: 0,
                count: invoices.length,
                averageSale: 0
            };

            invoices.forEach(invoice => {
                stats.totalSales += invoice.total;
                stats.totalPaid += invoice.paid;
                stats.totalDue += invoice.remaining;
            });

            stats.averageSale = stats.count > 0 ? stats.totalSales / stats.count : 0;

            // إضافة معلومات العميل لكل فاتورة
            let invoicesProcessed = 0;

            if (invoices.length === 0) {
                resolve({ invoices: [], stats });
                return;
            }

            invoices.forEach((invoice, index) => {
                const customerRequest = customersStore.get(invoice.customerId);

                customerRequest.onsuccess = function(event) {
                    const customer = event.target.result;
                    invoices[index].customerName = customer ? customer.name : 'غير معروف';

                    invoicesProcessed++;
                    if (invoicesProcessed === invoices.length) {
                        resolve({ invoices, stats });
                    }
                };

                customerRequest.onerror = function() {
                    invoices[index].customerName = 'غير معروف';

                    invoicesProcessed++;
                    if (invoicesProcessed === invoices.length) {
                        resolve({ invoices, stats });
                    }
                };
            });
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على تقرير المخزون
function getInventoryReport(productId = null, status = null) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['products'], 'readonly');
        const productsStore = transaction.objectStore('products');

        const request = productsStore.getAll();

        request.onsuccess = function(event) {
            let products = event.target.result;

            // تصفية المنتجات حسب المعرف
            if (productId) {
                products = products.filter(product => product.id === productId);
            }

            // تصفية المنتجات حسب حالة المخزون
            if (status) {
                switch (status) {
                    case 'low':
                        products = products.filter(product => product.stock <= product.minStock && product.stock > 0);
                        break;
                    case 'out':
                        products = products.filter(product => product.stock === 0);
                        break;
                    case 'available':
                        products = products.filter(product => product.stock > product.minStock);
                        break;
                }
            }

            // حساب الإحصائيات
            const stats = {
                totalValue: 0,
                count: products.length,
                lowStock: 0,
                outOfStock: 0
            };

            products.forEach(product => {
                // حساب قيمة المخزون
                const stock = product.stock || 0;
                const value = stock * product.purchasePrice;
                product.value = value;
                stats.totalValue += value;

                // حساب عدد المنتجات منخفضة المخزون والمنتهية
                if (stock === 0) {
                    stats.outOfStock++;
                } else if (stock <= product.minStock) {
                    stats.lowStock++;
                }
            });

            resolve({ products, stats });
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// ======== وظائف إدارة الموردين ========

// إضافة مورد جديد
function addSupplier(supplier) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['suppliers'], 'readwrite');
        const store = transaction.objectStore('suppliers');

        // التحقق من عدم وجود مورد بنفس الكود
        const codeIndex = store.index('code');
        const codeRequest = codeIndex.get(supplier.code);

        codeRequest.onsuccess = function(event) {
            if (event.target.result) {
                reject(new Error('يوجد مورد بنفس الكود'));
                return;
            }

            // إضافة المورد
            const request = store.add(supplier);

            request.onsuccess = function(event) {
                resolve(event.target.result); // إرجاع معرف المورد الجديد
            };

            request.onerror = function(event) {
                reject(event.target.error);
            };
        };

        codeRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على جميع الموردين
function getAllSuppliers() {
    return new Promise((resolve, reject) => {
        try {
            // التحقق من وجود قاعدة البيانات
            if (!db) {
                console.error('خطأ: قاعدة البيانات غير متاحة');
                return reject(new Error('قاعدة البيانات غير متاحة'));
            }

            // التحقق من وجود مخزن البيانات
            if (!db.objectStoreNames.contains('suppliers')) {
                console.error('خطأ: مخزن البيانات "suppliers" غير موجود');

                // إعادة تهيئة قاعدة البيانات
                return deleteDatabase()
                    .then(() => initDatabase())
                    .then(() => {
                        // إرجاع مصفوفة فارغة لأن قاعدة البيانات تم إعادة تهيئتها
                        resolve([]);
                    })
                    .catch(error => {
                        reject(error);
                    });
            }

            const transaction = db.transaction(['suppliers'], 'readonly');
            const store = transaction.objectStore('suppliers');
            const request = store.getAll();

            request.onsuccess = function(event) {
                const suppliers = event.target.result || [];
                console.log(`تم استرجاع ${suppliers.length} مورد من قاعدة البيانات`);
                resolve(suppliers);
            };

            request.onerror = function(event) {
                console.error('خطأ في استرجاع الموردين:', event.target.error);
                reject(event.target.error);
            };
        } catch (error) {
            console.error('خطأ غير متوقع في استرجاع الموردين:', error);
            reject(error);
        }
    });
}

// الحصول على مورد بواسطة المعرف
function getSupplierById(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['suppliers'], 'readonly');
        const store = transaction.objectStore('suppliers');
        const request = store.get(id);

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// تحديث مورد
function updateSupplier(supplier) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['suppliers'], 'readwrite');
        const store = transaction.objectStore('suppliers');

        // التحقق من عدم وجود مورد آخر بنفس الكود
        const codeIndex = store.index('code');
        const codeRequest = codeIndex.get(supplier.code);

        codeRequest.onsuccess = function(event) {
            const existingSupplier = event.target.result;
            if (existingSupplier && existingSupplier.id !== supplier.id) {
                reject(new Error('يوجد مورد آخر بنفس الكود'));
                return;
            }

            // تحديث المورد
            const request = store.put(supplier);

            request.onsuccess = function(event) {
                resolve(event.target.result);
            };

            request.onerror = function(event) {
                reject(event.target.error);
            };
        };

        codeRequest.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// حذف مورد
function deleteSupplier(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['suppliers', 'invoices'], 'readwrite');
        const suppliersStore = transaction.objectStore('suppliers');
        const invoicesStore = transaction.objectStore('invoices');

        // التحقق من عدم وجود فواتير مرتبطة بالمورد
        const request = invoicesStore.getAll();

        request.onsuccess = function(event) {
            const invoices = event.target.result;
            const supplierInvoices = invoices.filter(invoice =>
                invoice.type === 'purchase' && invoice.customerId === id
            );

            if (supplierInvoices.length > 0) {
                reject(new Error('لا يمكن حذف المورد لوجود فواتير مرتبطة به'));
                return;
            }

            // حذف المورد
            const deleteRequest = suppliersStore.delete(id);

            deleteRequest.onsuccess = function(event) {
                resolve(true);
            };

            deleteRequest.onerror = function(event) {
                reject(event.target.error);
            };
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// ======== وظائف إدارة المصروفات ========

// إضافة مصروف جديد
function addExpense(expense) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['expenses'], 'readwrite');
        const store = transaction.objectStore('expenses');

        // إضافة المصروف
        const request = store.add(expense);

        request.onsuccess = function(event) {
            resolve(event.target.result); // إرجاع معرف المصروف الجديد
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على جميع المصروفات
function getAllExpenses() {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['expenses'], 'readonly');
        const store = transaction.objectStore('expenses');
        const request = store.getAll();

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على مصروف بواسطة المعرف
function getExpenseById(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['expenses'], 'readonly');
        const store = transaction.objectStore('expenses');
        const request = store.get(id);

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// تحديث مصروف
function updateExpense(expense) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['expenses'], 'readwrite');
        const store = transaction.objectStore('expenses');

        // تحديث المصروف
        const request = store.put(expense);

        request.onsuccess = function(event) {
            resolve(event.target.result);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// حذف مصروف
function deleteExpense(id) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['expenses'], 'readwrite');
        const store = transaction.objectStore('expenses');

        // حذف المصروف
        const request = store.delete(id);

        request.onsuccess = function(event) {
            resolve(true);
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// الحصول على تقرير المصروفات
function getExpensesReport(fromDate, toDate, category = null) {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['expenses'], 'readonly');
        const store = transaction.objectStore('expenses');
        const request = store.getAll();

        request.onsuccess = function(event) {
            let expenses = event.target.result;

            // تصفية المصروفات حسب التاريخ
            if (fromDate) {
                const fromDateObj = new Date(fromDate);
                expenses = expenses.filter(expense => new Date(expense.date) >= fromDateObj);
            }

            if (toDate) {
                const toDateObj = new Date(toDate);
                toDateObj.setHours(23, 59, 59, 999); // نهاية اليوم
                expenses = expenses.filter(expense => new Date(expense.date) <= toDateObj);
            }

            // تصفية المصروفات حسب الفئة
            if (category) {
                expenses = expenses.filter(expense => expense.category === category);
            }

            // حساب الإحصائيات
            const stats = {
                totalExpenses: 0,
                count: expenses.length,
                byCategory: {}
            };

            expenses.forEach(expense => {
                stats.totalExpenses += expense.amount;

                // تجميع المصروفات حسب الفئة
                if (!stats.byCategory[expense.category]) {
                    stats.byCategory[expense.category] = 0;
                }

                stats.byCategory[expense.category] += expense.amount;
            });

            resolve({ expenses, stats });
        };

        request.onerror = function(event) {
            reject(event.target.error);
        };
    });
}

// تصدير الوظائف
window.db = {
    init: initDatabase,
    deleteDatabase: deleteDatabase,
    products: {
        add: addProduct,
        getAll: getAllProducts,
        getById: getProductById,
        update: updateProduct,
        delete: deleteProduct
    },
    customers: {
        add: addCustomer,
        getAll: getAllCustomers,
        getById: getCustomerById,
        update: updateCustomer,
        delete: deleteCustomer
    },
    suppliers: {
        add: addSupplier,
        getAll: getAllSuppliers,
        getById: getSupplierById,
        update: updateSupplier,
        delete: deleteSupplier
    },
    expenses: {
        add: addExpense,
        getAll: getAllExpenses,
        getById: getExpenseById,
        update: updateExpense,
        delete: deleteExpense
    },
    invoices: {
        add: addInvoice,
        getAll: getAllInvoices,
        getWithItems: getInvoiceWithItems,
        delete: deleteInvoice
    },
    inventory: {
        addTransaction: addInventoryTransaction,
        getTransactions: getInventoryTransactions
    },
    reports: {
        sales: getSalesReport,
        inventory: getInventoryReport,
        expenses: getExpensesReport
    },
    auth: {
        login: login
    }
};
