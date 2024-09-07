using System.ComponentModel;
using System.Reflection;
namespace ProductFamilyStateMachine;

public enum ProductState
{
    NotStarted,
    Editing,
    EditingDone
}
public enum StateTransition
{
    [Description("Edit Product 1")]
    NN_EN,

    [Description("Edit Product 2")]
    NN_NE,

    [Description("Edit Product 1")]
    NE_EE,

    [Description("Edit Product 2")]
    NE_NE,

    [Description("Finish Product 2")]
    NE_ND,

    [Description("Edit Product 1")]
    EN_EN,

    [Description("Edit Product 2")]
    EN_EE,

    [Description("Finish Product 1")]
    EN_DN,

    [Description("Edit Product 1")]
    ND_ED,

    [Description("Edit Product 1 and 2")]
    EE_EE,

    [Description("Finish Product 1")]
    EE_DE,

    [Description("Finish Product 2")]
    EE_ED,

    [Description("Edit Product 1")]
    ED_ED,

    [Description("Finish Product 1")]
    ED_DD,

    [Description("Edit Product 2")]
    DN_DE,

    [Description("Edit Product 2")]
    DE_DE,

    [Description("Finish Product 2")]
    DE_DD, 

    [Description("Product family editing finished!")]
    DD_DD,
}

public enum ProductFamilyState
{
    [Description("Both Not Started")]
    NN, // Both Not Started
    [Description("Product 1 Not Started, Product 2 Editing")]
    NE, // Product 1 Not Started, Product 2 Editing
    [Description("Product 1 Not Started, Product 2 EditingDone")]
    ND, // Product 1 Not Started, Product 2 EditingDone
    [Description("Product 1 Editing, Product 2 Not Started")]
    EN, // Product 1 Editing, Product 2 Not Started
    [Description("Both Editing")]
    EE, // Both Editing
    [Description("Product 1 Editing, Product 2 EditingDone")]
    ED, // Product 1 Editing, Product 2 EditingDone
    [Description("Product 1 EditingDone, Product 2 Not Started")]
    DN, // Product 1 EditingDone, Product 2 Not Started
    [Description("Product 1 EditingDone, Product 2 Editing")]
    DE, // Product 1 EditingDone, Product 2 Editing
    [Description("Both EditingDone")]
    DD  // Both EditingDone
}

class Product
{
    public ProductState State { get; set; } = ProductState.NotStarted;
    public string Description { get; set; } = string.Empty;
}
class ProductFamily
{
    public Product[] Products { get; set; } = new Product[] { new() { Description = "Product 1" }, new() { Description = "Product 2" } };
    private static Dictionary<ProductFamilyState, Dictionary<StateTransition, ProductFamilyState>> AllowedTransitions = new()
    {
        {
            ProductFamilyState.NN,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.NN_EN, ProductFamilyState.EN},
                {StateTransition.NN_NE, ProductFamilyState.NE},
            }
        },
        {
            ProductFamilyState.NE,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.NE_EE, ProductFamilyState.EE},
                {StateTransition.NE_NE, ProductFamilyState.NE},
                {StateTransition.NE_ND, ProductFamilyState.ND},
            }
        },
        {
            ProductFamilyState.EN,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.EN_EN, ProductFamilyState.EN},
                {StateTransition.EN_EE, ProductFamilyState.EE},
                {StateTransition.EN_DN, ProductFamilyState.DN},
            }
        },
        {
            ProductFamilyState.ND,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.ND_ED, ProductFamilyState.ED},
            }
        },
        {
            ProductFamilyState.EE,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.EE_EE, ProductFamilyState.EE},
                {StateTransition.EE_DE, ProductFamilyState.DE},
                {StateTransition.EE_ED, ProductFamilyState.ED},
            }
        },
        {
            ProductFamilyState.ED,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.ED_ED, ProductFamilyState.ED},
                {StateTransition.ED_DD, ProductFamilyState.DD},
            }
        },
        {
            ProductFamilyState.DN,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.DN_DE, ProductFamilyState.DE},
            }
        },
        {
            ProductFamilyState.DE,
            new Dictionary<StateTransition, ProductFamilyState>
            {
                {StateTransition.DE_DE, ProductFamilyState.DE},
                {StateTransition.DE_DD, ProductFamilyState.DD},
            }
        },
        {
            ProductFamilyState.DD,
            new Dictionary<StateTransition, ProductFamilyState>
            {
               {StateTransition.DD_DD, ProductFamilyState.DD},
            }
        },
    };

    public Dictionary<ProductFamilyState, Action> StateActions = new Dictionary<ProductFamilyState, Action>();
    public Dictionary<StateTransition, Action> StateTransitionActions = new Dictionary<StateTransition, Action>();

    public ProductFamilyState CurrentState = ProductFamilyState.NN;
    public List<StateTransition> GetAllowedTransitionsByCurrentState()
    {
        if (AllowedTransitions.ContainsKey(CurrentState))
        {
            return AllowedTransitions[CurrentState].Keys.ToList();
        }
        return new List<StateTransition>(); // Return an empty list if no transitions are allowed or if the state is not found.
    }

    public ProductFamily()
    {
        SetupStateTransitionsActions();
    }

    private void SetupStateTransitionsActions()
    {
        StateTransitionActions = new Dictionary<StateTransition, Action>()
        {
            { StateTransition.NN_EN, () => EditProduct(Products[0]) },
            { StateTransition.NN_NE, () => EditProduct(Products[1]) },
            { StateTransition.NE_EE, () => EditProduct(Products[0]) },
            { StateTransition.NE_NE, () => EditProduct(Products[1]) },
            { StateTransition.NE_ND, () => FinishProduct(Products[1]) },
            { StateTransition.EN_EN, () => EditProduct(Products[0]) },
            { StateTransition.EN_EE, () => EditProduct(Products[1]) },
            { StateTransition.EN_DN, () => FinishProduct(Products[0]) },
            { StateTransition.ND_ED, () => EditProduct(Products[0]) },
            { StateTransition.EE_EE, () => EditProducts() },
            { StateTransition.EE_DE, () => FinishProduct(Products[0]) },
            { StateTransition.EE_ED, () => FinishProduct(Products[1]) },
            { StateTransition.ED_ED, () => EditProduct(Products[0]) },
            { StateTransition.ED_DD, () => FinishProduct(Products[0]) },
            { StateTransition.DN_DE, () => EditProduct(Products[1]) },
            { StateTransition.DE_DE, () => EditProduct(Products[1]) },
            { StateTransition.DE_DD, () => FinishProduct(Products[1]) },
        };
    }
    public void ShowCurrentState()
    {
        Console.WriteLine("Current product states:");
        for (int i = 0; i < Products.Length; i++)
        {
            Console.WriteLine($"Product {i + 1}: {Products[i].Description}, State: {EnumHelper.GetEnumDescription(Products[i].State)}");
        }
        Console.WriteLine($"ProductFamily State: {CurrentState}");
        Console.WriteLine();
    }

    public void ChangeState(StateTransition transition)
    {
        // Check if the current state has any allowed transitions
        if (AllowedTransitions.ContainsKey(CurrentState))
        {
            var transitions = AllowedTransitions[CurrentState];

            // Check if the specified transition is allowed from the current state
            if (transitions.ContainsKey(transition))
            {
                // Transition is allowed, so update the state
                CurrentState = transitions[transition];
                Console.WriteLine($"State changed to {EnumHelper.GetEnumDescription(CurrentState)}");
            }
            else
            {
                // Transition is not allowed
                Console.WriteLine($"Transition {EnumHelper.GetEnumDescription(transition)} from {EnumHelper.GetEnumDescription(CurrentState)} is not allowed.");
            }
        }
        else
        {
            // No transitions are defined for the current state
            Console.WriteLine($"No transitions are defined for the current state: {EnumHelper.GetEnumDescription(CurrentState)}");
        }
    }


    public Action GetCurrentStateActionByTransition(StateTransition transition)
    {
        // This method returns the action associated with the current state, if any
        if (StateTransitionActions.TryGetValue(transition, out var action))
        {
            return action;
        }
        return null; // or a default action
    }

    public void EditProduct(Product product)
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

    public void EditProducts()
    {
        foreach (var product in Products) { 
           EditProduct(product);
        }

    }

    public void FinishProduct(Product product = null)
    {
        if (product==null)
        {
            Console.WriteLine($"Finishing editing for all products");
            Products[0].State = ProductState.EditingDone;
            Products[1].State = ProductState.EditingDone;
        } else
        {
            Console.WriteLine($"Finishing editing for {product.Description}");
            product.State = ProductState.EditingDone;
        }
    }
}


class Program
{
    static void Main()
    {
        ProductFamily productFamily = new();

        while (true)
        {
            productFamily.ShowCurrentState();
            Console.WriteLine("Select a state transition to execute:");
            var allowedTransitions = productFamily.GetAllowedTransitionsByCurrentState();

            if (allowedTransitions.Count == 0)
            {
                Console.WriteLine("No transitions are available from the current state.");
                break; // or continue, depending on your desired flow
            }

            for (int i = 0; i < allowedTransitions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {EnumHelper.GetEnumDescription(allowedTransitions[i])}");
            }

            Console.WriteLine();
            Console.Write("Enter your choice (or '0' to exit): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0)
            {
                break; // Exit the loop if the user enters '0' or an invalid number
            }

            if (choice > 0 && choice <= allowedTransitions.Count)
            {
                var transition = allowedTransitions[choice - 1];
                productFamily.ChangeState(transition);
                // Check if the current state has an associated action and invoke it
                var action = productFamily.GetCurrentStateActionByTransition(transition);
                action?.Invoke();
            }

            else
            {
                Console.WriteLine("Invalid selection. Please try again.");
            }

            if (productFamily.CurrentState == ProductFamilyState.DD)
            {
                productFamily.ShowCurrentState();
                Console.ReadLine();
                break;
            }
        };
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