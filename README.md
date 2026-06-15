# Unity Interactive Book Library

A toolset for **Unity 6** designed for the mass production and rapid prototyping of multi-branching, interactive digital gamebooks. It enables designers to visually create non-linear stories with dynamic choices, global variables, and inventory systems without writing code, while maintaining full flexibility for programmers.

---

## 🚀 Core Features

* **Visual Story Editor (`BookGraph`):** Design non-linear narrative paths using an intuitive node-based interface powered by the `xNode` framework. 
* **Automated Asset Sync:** Adding a new paragraph node automatically generates its corresponding page prefab variant with a unique index. Deleting a node automatically cleans up the unneeded files from the disk to keep the project clean.
* **Global Variables (`GlobalVariables`):** Built-in support for tracking flags and counters (`int`, `float`, `bool`, `string`) managed via a dedicated editor window equipped with a search bar.
* **Dynamic Conditions & Actions:** Lock or reveal specific choices based on variable states (e.g., checking stats or item ownership) and automatically modify parameters as a result of player decisions.
* **Inventory Module (`InventoryVisuals`):** A visual inventory system linked directly to global variables. Any change in an item's quantity automatically triggers an update in the user interface.
* **Polymorphic Content Architecture (`IPageContent`):** Pages act as universal containers. Since content logic is decoupled via interfaces, programmers can easily create entirely new content types (text, imagery, multimedia) that designers can freely assemble in the inspector.

---

## 📖 Guide for Designers

### 1. Setting Up the Story Structure
1. In the *Project* window, right-click and select **Create -> Book Graph**.
2. Open the created graph file, right-click the workspace, and add a **StartNode** and several **ParagraphNodes**.
3. Define your choices inside the nodes and connect the output ports to the subsequent paragraph nodes. One choice leads to exactly one destination page.

### 2. Managing Inventory and Variables
1. Open the global variables editor, navigate to the **Add New Variable** section, and define initial values for your flags or counters.
2. To create an item, right-click in the project window and select **Create -> Inventory -> InventoryBaseItemsData**.
3. Add an item to the list, link it to its corresponding `int` variable, assign a `Sprite` icon, and enter a `Display Name`.

### 3. Scene Setup and Hot-Swapping Book Visuals
1. Place the `BookManager` component onto a GameObject in your scene and assign your `BookGraph` asset to it.
2. **Interchangeable Page Flipping & Visual Styles:** The visual presentation and page turning animations (such as the physical page curl effect or alternative transition styles) are completely decoupled from the underlying narrative logic. You can effortlessly swap out templates and animation components inside the `BookManager` configuration slots without breaking your dialogues, choices, or variable structures.

---

## 🛠️ Requirements & Dependencies

* **Unity Version:** `6000.2.2` or higher.
* **External Dependencies:**
  * **`xNode`** – Backend for the node graph system.
  * **`SaintsField`** & **`Scene Reference Attribute`** – Advanced inspector formatting attributes.
  * **`Unity SerializeReferenceExtensions`** (`SubclassSelector`) – Polymorphic data serialization for interfaces right inside the Unity Inspector.

---
