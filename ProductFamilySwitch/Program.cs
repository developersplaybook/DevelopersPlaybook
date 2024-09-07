using System.ComponentModel;
using System.Reflection;

namespace ProductFamilySwitch;

enum ProductState
{
    [Description("Editing not started")]
    NotStarted,
    [Description("Editing started")]
    Editing,
    [Description("Editing finished")]
    EditingDone
}

enum ProductFamilyState
{
    NN, // Both Not Started
    NE, // Product 1 Not Started, Product 2 Editing
    ND, // Product 1 Not Started, Product 2 EditingDone
    EN, // Product 1 Editing, Product 2 Not Started
    EE, // Both Editing
    ED, // Product 1 Editing, Product 2 EditingDone
    DN, // Product 1 EditingDone, Product 2 Not Started
    DE, // Product 1 EditingDone, Product 2 Editing
    DD  // Both EditingDone
}

class Product
{
    public string Description { get; set; } = string.Empty;
    public ProductState State { get; set; } = ProductState.NotStarted;
}

class ProductFamily
{
    public Product[] Products { get; set; } = [new Product { Description = "Product 1" }, new Product { Description = "Product 2" }];

    public ProductFamilyState GetFamilyState()
    {
        var state1 = Products[0].State;
        var state2 = Products[1].State;

        return (state1, state2) switch
        {
            (ProductState.NotStarted, ProductState.NotStarted) => ProductFamilyState.NN,
            (ProductState.NotStarted, ProductState.Editing) => ProductFamilyState.NE,
            (ProductState.NotStarted, ProductState.EditingDone) => ProductFamilyState.ND,
            (ProductState.Editing, ProductState.NotStarted) => ProductFamilyState.EN,
            (ProductState.Editing, ProductState.Editing) => ProductFamilyState.EE,
            (ProductState.Editing, ProductState.EditingDone) => ProductFamilyState.ED,
            (ProductState.EditingDone, ProductState.NotStarted) => ProductFamilyState.DN,
            (ProductState.EditingDone, ProductState.Editing) => ProductFamilyState.DE,
            (ProductState.EditingDone, ProductState.EditingDone) => ProductFamilyState.DD,
            _ => throw new InvalidOperationException("Unknown state")
        };
    }

    public void ShowCurrentFamilyState()
    {
        Console.WriteLine();
        Console.WriteLine("Current States:");
        for (int i = 0; i < Products.Length; i++)
        {
            Console.WriteLine($"Product {i + 1}: {Products[i].Description}, State: {EnumHelper.GetEnumDescription(Products[i].State)}");
        }
        Console.WriteLine($"ProductFamily State: {GetFamilyState()}");
        Console.WriteLine();
    }
}

class Program
{
    static void Main(string[] args)
    {
        ProductFamily productFamily = new();
        bool continueEditing = true;

        while (continueEditing)
        {
            Console.Clear();
            productFamily.ShowCurrentFamilyState();
            Console.WriteLine("Enter your choice:");

            ProductFamilyState familyState = productFamily.GetFamilyState();
            DisplayOptions(familyState);

            char choice = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (choice == '0')
            {
                break;
            }

            HandleChoice(choice, familyState, productFamily, ref continueEditing);
        }

        Console.WriteLine("Exiting...");
    }

    private static void DisplayOptions(ProductFamilyState state)
    {
        switch (state)
        {
            case ProductFamilyState.NN:
                Console.WriteLine("1: Edit Product 1");   //NN_EN
                Console.WriteLine("2: Edit Product 2");   //NN_NE
                break;
            case ProductFamilyState.NE:
                Console.WriteLine("1: Edit Product 1");   //NE_EE
                Console.WriteLine("2: Edit Product 2");   //NE_NE
                Console.WriteLine("3: Finish Product 2"); //NE_ND
                break;
            case ProductFamilyState.EN:
                Console.WriteLine("1: Edit Product 1");   //EN_EN
                Console.WriteLine("2: Edit Product 2");   //EN_EE
                Console.WriteLine("3: Finish Product 1"); //EN_DN
                break;
            case ProductFamilyState.ND:
                Console.WriteLine("1: Edit Product 1");   //ND_ED 
                break;
            case ProductFamilyState.EE:
                Console.WriteLine("1: Edit Product 1");   //EE_EE
                Console.WriteLine("2: Edit Product 2");   //EE_EE 
                Console.WriteLine("3: Finish Product 1"); //EE_DE
                Console.WriteLine("4: Finish Product 2"); //EE_ED
                break;
            case ProductFamilyState.ED:
                Console.WriteLine("1: Edit Product 1");   //ED_ED
                Console.WriteLine("2: Finish Product 1"); //ED_DD
                break;
            case ProductFamilyState.DN:
                Console.WriteLine("1: Edit Product 2");   //DN_DE
                break;
            case ProductFamilyState.DE:
                Console.WriteLine("1: Edit Product 2");   //DE_DE
                Console.WriteLine("2: Finish Product 2"); //DE_DD
                break;
            case ProductFamilyState.DD:
                Console.WriteLine("Product family finished editing");
                break;
        }
        Console.WriteLine("0: Exit");
    }

    private static void HandleChoice(char choice, ProductFamilyState state, ProductFamily productFamily, ref bool continueEditing)
    {
        switch (state)
        {
            case ProductFamilyState.NN:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[0]);
                }
                else if (choice == '2')
                {
                    EditProduct(productFamily.Products[1]);
                }
                break;
            case ProductFamilyState.NE:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[0]);
                }
                else if (choice == '2')
                {
                    EditProduct(productFamily.Products[1]);
                }
                else if (choice == '3')
                {
                    FinishProduct(productFamily.Products[1]);
                }
                break;
            case ProductFamilyState.EN:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[0]);
                }
                else if (choice == '2')
                {
                    EditProduct(productFamily.Products[1]);
                }
                else if (choice == '3')
                {
                    FinishProduct(productFamily.Products[0]);
                }
                break;
            case ProductFamilyState.ND:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[0]);
                }
                break;
            case ProductFamilyState.EE:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[0]);
                }
                else if (choice == '2')
                {
                    EditProduct(productFamily.Products[1]);
                }
                else if (choice == '3')
                {
                    FinishProduct(productFamily.Products[0]);
                }
                else if (choice == '4')
                {
                    FinishProduct(productFamily.Products[1]);
                }
                break;
            case ProductFamilyState.ED:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[0]);
                }
                else if (choice == '2')
                {
                    FinishProduct(productFamily.Products[0]);
                }
                break;
            case ProductFamilyState.DN:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[1]);
                }
                break;
            case ProductFamilyState.DE:
                if (choice == '1')
                {
                    EditProduct(productFamily.Products[1]);
                }
                else if (choice == '2')
                {
                    FinishProduct(productFamily.Products[1]);
                }
                break;
            case ProductFamilyState.DD:
                continueEditing = false;
                break;
        }
    }

    private static void EditProduct(Product product)
    {
        Console.WriteLine();
        Console.WriteLine("Edit the product description (type '0' to finish): ");
        product.State = ProductState.Editing;

        while (true)
        {
            Console.WriteLine($"Description: {product.Description}");
            string userInput = Console.ReadLine();
            if (userInput == "0")
            {
                break;
            }
            product.Description = userInput;
            Console.WriteLine("Description updated. Continue editing or type '0' to finish:");
        }
    }

    private static void FinishProduct(Product product)
    {
        Console.WriteLine($"Finishing editing for {product.Description}");
        product.State = ProductState.EditingDone;
    }
}

public static class EnumHelper
{
    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes != null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }
        else
        {
            return value.ToString();
        }
    }
}
