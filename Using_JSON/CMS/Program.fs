open System
open System.IO
open System.Text.Json
open System.Windows.Forms
open System.Collections.Generic

// Define a record type for contact data
type Contact = {
    Name: string
    Number: string
    Email: string
}

[<EntryPoint>]
let main argv =
    // File path for the JSON file
    let filePath = "contacts.json"

    // Load contacts from the JSON file
    let loadContacts () =
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            JsonSerializer.Deserialize<List<Contact>>(json)
        else
            new List<Contact>()

    // Save contacts to the JSON file
    let saveContacts (contacts: List<Contact>) =
        let json = JsonSerializer.Serialize(contacts)
        File.WriteAllText(filePath, json)

    // Main form`
    let form = new Form(Text = "Contact Management System", Width = 890, Height = 330)

    // DataGridView to display data
    let dataGridView = new DataGridView(Width = 350, Height = 200, Top = 20, Left = 500)
    dataGridView.ReadOnly <- true
    dataGridView.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill

    // Load data into DataGridView
    let loadData () =
        let contacts = loadContacts() |> Seq.toList
        dataGridView.DataSource <- contacts |> List.toArray

    // Refresh button
    let refreshButton = new Button(Text = "Refresh", Top = 230, Left = 620, Width = 100)
    refreshButton.Click.Add(fun _ -> loadData())

    // UI elements
    let nameLabel = new Label(Text = "Name:", AutoSize = true, Top = 110, Left = 20)
    let nameTextBox = new TextBox(Width = 280, Top = 110, Left = 80)

    let numberLabel = new Label(Text = "Number:", AutoSize = true, Top = 150, Left = 20)
    let numberTextBox = new TextBox(Width = 280, Top = 150, Left = 80)

    let emailLabel = new Label(Text = "Email:", AutoSize = true, Top = 190, Left = 20)
    let emailTextBox = new TextBox(Width = 280, Top = 190, Left = 80)

    // Search by number
    let searchNLabel = new Label(Text = "Search by Number:", AutoSize = true, Top = 20, Left = 20)
    let searchNTextBox = new TextBox(Width = 210, Top = 20, Left = 150, MaxLength = 11)
    let searchNumButton = new Button(Text = "Search", Top = 20, Left = 370, Width = 100)

    searchNumButton.Click.Add(fun _ ->
        let searchValue = searchNTextBox.Text
        let contacts = loadContacts() |> Seq.toList
        match contacts |> List.tryFind (fun c -> c.Number = searchValue) with
        | Some contact ->
            nameTextBox.Text <- contact.Name
            numberTextBox.Text <- contact.Number
            emailTextBox.Text <- contact.Email
        | None -> MessageBox.Show("This number does not exist.") |> ignore
    )

    // Search by name
    let searchNameLabel = new Label(Text = "Search by Name:", AutoSize = true, Top = 60, Left = 20)
    let searchNameTextBox = new TextBox(Width = 210, Top = 60, Left = 150)
    let searchNameButton = new Button(Text = "Search", Top = 60, Left = 370, Width = 100)

    searchNameButton.Click.Add(fun _ ->
        let searchValue = searchNameTextBox.Text
        let contacts = loadContacts() |> Seq.toList
        match contacts |> List.tryFind (fun c -> c.Name.Equals(searchValue, StringComparison.OrdinalIgnoreCase)) with
        | Some contact ->
            nameTextBox.Text <- contact.Name
            numberTextBox.Text <- contact.Number
            emailTextBox.Text <- contact.Email
        | None -> MessageBox.Show("This name does not exist.") |> ignore
    )

    // Add button
    let addButton = new Button(Text = "Add", Top = 230, Left = 260, Width = 100)
    addButton.Click.Add(fun _ ->
        let name = nameTextBox.Text
        let email = emailTextBox.Text
        let number = numberTextBox.Text
        if not (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(number)) then
            let contacts = loadContacts() |> Seq.toList
            if contacts |> List.exists (fun c -> c.Number = number) then
                MessageBox.Show("A contact with this number already exists.") |> ignore
            else
                let newContact = { Name = name; Number = number; Email = email }
                let updatedContacts = new List<Contact>(contacts)
                updatedContacts.Add(newContact)
                saveContacts(updatedContacts)
                MessageBox.Show("Contact added successfully.") |> ignore
                loadData()
        else
            MessageBox.Show("Please enter valid data.") |> ignore
    )

    // Edit button
    let editButton = new Button(Text = "Edit", Top = 230, Left = 140, Width = 100)
    editButton.Click.Add(fun _ ->
        let name = nameTextBox.Text
        let email = emailTextBox.Text
        let number = numberTextBox.Text
        if not (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(number)) then
            let contacts = loadContacts() |> Seq.toList
            match contacts |> List.tryFindIndex (fun c -> c.Number = number) with
            | Some index ->
                let updatedContacts = new List<Contact>(contacts)
                updatedContacts.[index] <- { Name = name; Number = number; Email = email }
                saveContacts(updatedContacts)
                MessageBox.Show("Contact updated successfully.") |> ignore
                loadData()
            | None -> MessageBox.Show("Contact not found.") |> ignore
        else
            MessageBox.Show("Please enter valid data.") |> ignore
    )

    // Delete button
    let deleteButton = new Button(Text = "Delete", Top = 230, Left = 20, Width = 100)
    deleteButton.Click.Add(fun _ ->
        let number = numberTextBox.Text
        if not (String.IsNullOrWhiteSpace(number)) then
            let contacts = loadContacts() |> Seq.toList
            let updatedContacts = contacts |> List.filter (fun c -> c.Number <> number)
            if updatedContacts.Length < contacts.Length then
                saveContacts(new List<Contact>(updatedContacts))
                MessageBox.Show("Contact deleted successfully.") |> ignore
                loadData()
            else
                MessageBox.Show("Contact not found.") |> ignore
        else
            MessageBox.Show("Please enter a valid number.") |> ignore
    )

    // Clear button
    let clearButton = new Button(Text = "Clear", Top = 110, Left = 370, Width = 100)
    clearButton.Click.Add(fun _ ->
        nameTextBox.Text <- ""
        numberTextBox.Text <- ""
        emailTextBox.Text <- ""
    )

    // Add controls to the form
    form.Controls.AddRange(
        [| dataGridView; refreshButton;
           searchNLabel; searchNTextBox; searchNumButton;
           searchNameLabel; searchNameTextBox; searchNameButton;
           nameLabel; nameTextBox;
           numberLabel; numberTextBox;
           emailLabel; emailTextBox;
           addButton; editButton; deleteButton; clearButton |]
    )

    // Initial data load
    loadData()

    // Run application
    Application.Run(form)
    0
