using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Configuration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace Kursach
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection conn;
        SqlCommand cmd;
        SqlCommand mainCmd;
        int index;
        SqlDataAdapter adapter;
        DataTable dt;
        DataTable searchTable;
        DataTable addTable1= new DataTable();
        DataTable deleteTable1= new DataTable();
        DataTable addTable2= new DataTable();
        DataTable deleteTable2= new DataTable();
        DataTable addTable = new DataTable();
        public MainWindow()
        {
            index = 0;
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            InitializeComponent();
            dt = new DataTable();
            mainCmd = new SqlCommand("select dbo.Music.[Name], dbo.Music.RecCondition, dbo.Music.IDMusic from dbo.Music group by dbo.Music.[Name], dbo.Music.RecCondition, dbo.Music.IDMusic", conn);
            adapter = new SqlDataAdapter(mainCmd);
            adapter.Fill(dt);
            musicChanged();
        }
        //Оновлюємо відображення в основній сторінці
        private void musicChanged()
        {
            if(musicShow.Visibility == Visibility.Visible)
            {
                
                dt = new DataTable();
                mainCmd = new SqlCommand("select dbo.Music.[Name], dbo.Music.RecCondition, dbo.Music.IDMusic from dbo.Music group by dbo.Music.[Name], dbo.Music.RecCondition, dbo.Music.IDMusic", conn);
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                musicName.Content = dt.Rows[index][0];
                musicRec.Content = dt.Rows[index][1];
                cmd = new SqlCommand("select dbo.[Group].Name, dbo.[Group].IDGroup  FROM dbo.[Group] INNER JOIN dbo.MusicGroup ON dbo.[Group].IDGroup = dbo.MusicGroup.IDGroup INNER JOIN dbo.Music ON dbo.MusicGroup.IDMusic = dbo.Music.IDMusic where dbo.Music.IDMusic = '" + dt.Rows[index][2] + "' GROUP BY dbo.[Group].Name, dbo.Music.IDMusic, dbo.[Group].IDGroup", conn);
                adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                groupListDataGrid.ItemsSource = table.DefaultView;
                cmd = new SqlCommand("select dbo.Genre.Name, dbo.Genre.IDGenre FROM dbo.Genre INNER JOIN dbo.MusicGenre ON dbo.Genre.IDGenre = dbo.MusicGenre.IDGenre INNER JOIN dbo.Music ON dbo.MusicGenre.IDMusic = dbo.Music.IDMusic where dbo.Music.IDMusic = '" + dt.Rows[index][2] + "' GROUP BY dbo.Genre.Name, dbo.Music.IDMusic, dbo.Genre.IDGenre", conn);
                adapter = new SqlDataAdapter(cmd);
                DataTable data = new DataTable();
                adapter.Fill(data);
                genreListDataGrid.ItemsSource = data.DefaultView;
            }
            else if(coverShow.Visibility == Visibility.Visible)
            {
                dt = new DataTable();
                mainCmd = new SqlCommand("select dbo.Cover.Name, dbo.Cover.IDCover from dbo.Cover group by dbo.Cover.Name, dbo.Cover.IDCover", conn);
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                coverName.Content = dt.Rows[index][0];
                cmd = new SqlCommand("select Cover.Retail_Price, Cover.N_This_Year, Cover.N_Last_Year, Cover.N_All_Time from Cover Where IDCover = " + dt.Rows[index][1], conn);
                adapter=new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                coverPrice.Content = "Costs: "+table.Rows[0][0];
                coverSellsThisYear.Content = "This year: " + table.Rows[0][1];
                coverSellsLastYear.Content = "Last year: " + table.Rows[0][2];
                coverSellsAllTime.Content = "All time: "+table.Rows[0][3];
                cmd = new SqlCommand("SELECT dbo.Music.Name as Name FROM dbo.Music INNER JOIN dbo.Music_Cover ON dbo.Music.IDMusic = dbo.Music_Cover.IDMusic INNER JOIN dbo.Cover ON dbo.Music_Cover.IDCover = dbo.Cover.IDCover Where Cover.IDCover = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                table = new DataTable();
                adapter.Fill(table);
                musicGridInCover.ItemsSource = table.DefaultView;
            }
            else if(groupShow.Visibility== Visibility.Visible)
            {
                dt = new DataTable();
                mainCmd = new SqlCommand("select dbo.[Group].Name, dbo.[Group].IDGroup, dbo.[Group].Type from dbo.[Group] group by dbo.[Group].Name, dbo.[Group].IDGroup, dbo.[Group].Type", conn);
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                groupName.Content = dt.Rows[index][0];
                groupType.Content = "Type: " + dt.Rows[index][2];
                cmd = new SqlCommand("SELECT dbo.Musician.Name+' '+dbo.Musician.Surname FROM dbo.[Group] INNER JOIN dbo.Musician ON dbo.[Group].IDGroup = dbo.Musician.IDGroup WHERE dbo.[Group].IDGroup = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                groupMusicianList.ItemsSource = table.DefaultView;
            }
            else if(musicianShow.Visibility== Visibility.Visible)
            {
                dt = new DataTable();
                DataTable table;
                mainCmd = new SqlCommand("select dbo.Musician.Name, dbo.Musician.IDMusician, dbo.Musician.Surname, dbo.Musician.IDGroup from dbo.Musician group by dbo.Musician.Name, dbo.Musician.IDMusician, dbo.Musician.Surname, dbo.Musician.IDGroup", conn);
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                musicianName.Content = dt.Rows[index][0].ToString()+" "+dt.Rows[index][2].ToString();
                if (dt.Rows[index][3].ToString() != "")
                {
                    cmd = new SqlCommand("SELECT dbo.[Group].Name FROM dbo.[Group] INNER JOIN dbo.Musician ON dbo.[Group].IDGroup = dbo.Musician.IDGroup WHERE dbo.Musician.IDMusician = " + dt.Rows[index][1], conn);
                    adapter = new SqlDataAdapter(cmd);
                    table = new DataTable();
                    adapter.Fill(table);
                    musicianGroup.Content = table.Rows[0][0];
                }
                else
                {
                    musicianGroup.Content = "";
                }
                cmd = new SqlCommand("SELECT dbo.Instrument.Name AS Instrument FROM dbo.Musician INNER JOIN dbo.MusicianInstrument ON dbo.Musician.IDMusician = dbo.MusicianInstrument.IDMusician INNER JOIN dbo.Instrument ON dbo.MusicianInstrument.IDInstrument = dbo.Instrument.IDInstrument WHERE dbo.Musician.IDMusician = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                table = new DataTable();
                adapter.Fill(table);
                cmd = new SqlCommand("SELECT dbo.Occupation.Name AS Occupation FROM dbo.Musician INNER JOIN dbo.MusicianOccupation ON dbo.Musician.IDMusician = dbo.MusicianOccupation.IDMusician INNER JOIN dbo.Occupation ON dbo.MusicianOccupation.IDOccupation = dbo.Occupation.IDOccupation WHERE dbo.Musician.IDMusician = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                DataTable dtable = new DataTable();
                adapter.Fill(dtable);
                musicianOccupation.ItemsSource = dtable.DefaultView;
                musicianInstrument.ItemsSource = table.DefaultView;
            }
            else if (groupShow.Visibility == Visibility.Visible)
            {
                dt = new DataTable();
                mainCmd = new SqlCommand("select dbo.[Group].Name, dbo.[Group].IDGroup, dbo.[Group].Type from dbo.[Group] group by dbo.[Group].Name, dbo.[Group].IDGroup, dbo.[Group].Type", conn);
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                groupName.Content = dt.Rows[index][0];
                groupType.Content = "Type: " + dt.Rows[index][2];
                cmd = new SqlCommand("SELECT dbo.Musician.Name+' '+dbo.Musician.Surname FROM dbo.[Group] INNER JOIN dbo.Musician ON dbo.[Group].IDGroup = dbo.Musician.IDGroup" +
                    " WHERE dbo.[Group].IDGroup = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                groupMusicianList.ItemsSource = table.DefaultView;
            }
            else if (diskShow.Visibility == Visibility.Visible)
            {
                dt = new DataTable();
                mainCmd = new SqlCommand("SELECT dbo.Plate.IDPlate as ID, dbo.Cover.Name AS Cover, dbo.Plate.N_In_Stock as Stock, dbo.Plate.Release_Time as Release," +
                    "  dbo.Plate.Wholesale_price as Price, dbo.Company.Name as Seller, (select dbo.Company.Name from dbo.Company where IDCompany=IDCompany_Creator)as Creator " +
                    "FROM dbo.Plate INNER JOIN dbo.Cover ON dbo.Plate.IDCover = dbo.Cover.IDCover inner join dbo.Company ON dbo.Plate.IDCompany_Seller = dbo.Company.IDCompany " +
                    "group by Cover.Name, dbo.Plate.N_In_Stock, dbo.Plate.Release_Time, dbo.Plate.Wholesale_price, dbo.Company.Name, dbo.Plate.IDCompany_Creator, dbo.Plate.IDPlate", conn);
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                DataTable table = new DataTable();
                adapter.Fill(table);
                selectedDiskList.ItemsSource = table.DefaultView;
                disksInfo.ItemsSource = table.DefaultView;
            }
        }
        private void leftItem(object sender, MouseButtonEventArgs e)
        {
            index=(index-1+dt.Rows.Count)%dt.Rows.Count;
            musicChanged();
        }
        private void rightItem(object sender, MouseButtonEventArgs e)
        {
            index=(index+1) % dt.Rows.Count;
            musicChanged();
        }
        private void disksInfo_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }
        private void disksInfo_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //MessageBox.Show((selectedDiskList.SelectedValue==((DataRowView)(e.AddedCells[0].Item)).Row[0]).ToString());
            try
            {
                selectedDiskList.SelectedItem = (DataRowView)(e.AddedCells[0].Item);
            }
            catch { }

        }
        //Посилання на інші вікна
        private void groupListDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (e.AddedCells.Count != 0)
                {
                    index = groupListDataGrid.Items.IndexOf(e.AddedCells[0].Item);
                    index = Convert.ToInt32(((DataRowView)groupListDataGrid.Items[index]).Row[1]);
                    dt = new DataTable();
                    mainCmd = new SqlCommand("select dbo.[Group].Name, dbo.[Group].IDGroup, dbo.[Group].Type from dbo.[Group] group by dbo.[Group].Name, dbo.[Group].IDGroup, dbo.[Group].Type", conn);
                    adapter = new SqlDataAdapter(mainCmd);
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dt.Rows[i][1]) == index)
                        {
                            index = i;
                            break;
                        }
                    }
                    showTab(groupShow, 2);
                    musicChanged();
                }
            }
            else if(coverShow.Visibility == Visibility.Visible)
            {
                if (e.AddedCells.Count != 0)
                {
                    
                    cmd = new SqlCommand("SELECT dbo.Music.IDMusic FROM dbo.Music INNER JOIN dbo.Music_Cover ON dbo.Music.IDMusic = dbo.Music_Cover.IDMusic INNER JOIN dbo.Cover ON dbo.Music_Cover.IDCover = dbo.Cover.IDCover Where Cover.IDCover = " + dt.Rows[index][1], conn);
                    adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    index = musicGridInCover.Items.IndexOf(e.AddedCells[0].Item);
                    dt = new DataTable();
                    mainCmd = new SqlCommand("select dbo.Music.[Name], dbo.Music.RecCondition, dbo.Music.IDMusic from dbo.Music group by dbo.Music.[Name], dbo.Music.RecCondition, dbo.Music.IDMusic", conn);
                    adapter = new SqlDataAdapter(mainCmd);
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dt.Rows[i][2]) == Convert.ToInt32(table.Rows[index][0]))
                        {
                            index = i;
                            break;
                        }
                    }
                    showTab(musicShow, 0);
                    musicChanged();
                }
            }
            else if(groupShow.Visibility == Visibility.Visible)
            {

                if (e.AddedCells.Count != 0)
                {

                    cmd = new SqlCommand("SELECT dbo.Musician.IDMusician FROM dbo.[Group] INNER JOIN dbo.Musician ON dbo.[Group].IDGroup = dbo.Musician.IDGroup WHERE dbo.[Group].IDGroup = " + dt.Rows[index][1], conn);
                    adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    index = groupMusicianList.Items.IndexOf(e.AddedCells[0].Item);
                    dt = new DataTable();
                    mainCmd = new SqlCommand("select dbo.Musician.Name, dbo.Musician.IDMusician, dbo.Musician.Surname, dbo.Musician.IDGroup from dbo.Musician group by dbo.Musician.Name, dbo.Musician.IDMusician, dbo.Musician.Surname, dbo.Musician.IDGroup", conn);
                    adapter = new SqlDataAdapter(mainCmd);
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dt.Rows[i][1]) == Convert.ToInt32(table.Rows[index][0]))
                        {
                            index = i;
                            break;
                        }
                    }
                    showTab(musicianShow, 3);
                    musicChanged();
                }
                
            }
        }


        #region Search
        //Введення в пошук ключових слів
        private void musicSearchText_KeyUp(object sender, KeyEventArgs e)//поиск с компаниями не закончен
        {
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (musicSearchText.Text != "")
                {
                    searchTable = new DataTable();
                    SqlCommand command = new SqlCommand(mainCmd.CommandText + " Having (dbo.Music.Name Like '" + musicSearchText.Text + "%')", conn);
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(searchTable);
                    musicSearchList.Items.Clear();
                    for (int i = 0; i < searchTable.Rows.Count; i++)
                    {
                        musicSearchList.Items.Add(searchTable.Rows[i][0]);
                    }
                    musicSearchList.IsDropDownOpen = true;
                }
                else
                {
                    musicSearchList.IsDropDownOpen = false;
                }
            }
            else if (coverShow.Visibility == Visibility.Visible)
            {
                if (coverSearchText.Text != "")
                {
                    searchTable = new DataTable();
                    SqlCommand command = new SqlCommand(mainCmd.CommandText + " Having (dbo.Cover.Name Like '" + coverSearchText.Text + "%')", conn);
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(searchTable);
                    coverSearchList.Items.Clear();
                    for (int i = 0; i < searchTable.Rows.Count; i++)
                    {
                        coverSearchList.Items.Add(searchTable.Rows[i][0]);
                    }
                    coverSearchList.IsDropDownOpen = true;
                }
                else
                {
                    coverSearchList.IsDropDownOpen = false;
                }
            }
            else if (groupShow.Visibility == Visibility.Visible)
            {
                if (groupSearchText.Text != "")
                {
                    searchTable = new DataTable();
                    SqlCommand command = new SqlCommand(mainCmd.CommandText + " Having (dbo.[Group].Name Like '" + groupSearchText.Text + "%')", conn);
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(searchTable);
                    groupSearchList.Items.Clear();
                    for (int i = 0; i < searchTable.Rows.Count; i++)
                    {
                        groupSearchList.Items.Add(searchTable.Rows[i][0]);
                    }
                    groupSearchList.IsDropDownOpen = true;
                }
                else
                {
                    groupSearchList.IsDropDownOpen = false;
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (musicianSearchText.Text != "")
                {
                    searchTable = new DataTable();
                    SqlCommand command;
                    if (musicianSearchText.Text.Split(' ').Length == 1)
                    {
                        command = new SqlCommand(mainCmd.CommandText + " Having (dbo.Musician.Name Like '" + musicianSearchText.Text.Split(' ')[0].Replace("'", "''") + "%')", conn);
                    }
                    else
                    {
                        command = new SqlCommand(mainCmd.CommandText + " Having (dbo.Musician.Name Like '" + musicianSearchText.Text.Split(' ')[0].Replace("'", "''") + "%' and dbo.Musician.Surname Like '" + musicianSearchText.Text.Split(' ')[1].Replace("'", "''") + "%')", conn);
                    }
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(searchTable);
                    musicianSearchList.Items.Clear();
                    for (int i = 0; i < searchTable.Rows.Count; i++)
                    {
                        musicianSearchList.Items.Add(searchTable.Rows[i][0] + " " + searchTable.Rows[i][2]);
                    }
                    musicianSearchList.IsDropDownOpen = true;
                }
                else
                {
                    musicianSearchList.IsDropDownOpen = false;
                }
            }
            else if (diskShow.Visibility == Visibility.Visible)
            {
                if (searchDiskTb.Text != "")
                {
                    searchTable = new DataTable();
                    SqlCommand command = new SqlCommand();
                    switch (searchDiskCategory.SelectedIndex)
                    {
                        case 0:
                            command = new SqlCommand(mainCmd.CommandText + " Having dbo.Cover.Name Like '" + searchDiskTb.Text.Replace("'", "''") + "%'", conn);
                            break;
                        case 1:
                            command = new SqlCommand("SELECT dbo.Plate.IDPlate as ID, dbo.Cover.Name AS Cover, dbo.Plate.N_In_Stock as Stock, dbo.Plate.Release_Time as Release, dbo.Plate.Wholesale_price as Price, " +
                                                      "dbo.Company.Name as Seller, (select dbo.Company.Name from dbo.Company where IDCompany = IDCompany_Creator) as Creator " +
                                                      "FROM dbo.Plate INNER JOIN dbo.Cover ON dbo.Plate.IDCover = dbo.Cover.IDCover inner join dbo.Company ON dbo.Plate.IDCompany_Seller = dbo.Company.IDCompany " +
                                                      "where IDCompany_Seller in (select IDCompany from dbo.Company group by IDCompany, Name having Name like '" + searchDiskTb.Text.Replace("'", "''") + "%') " +
                                                      "group by Cover.Name, dbo.Plate.N_In_Stock, dbo.Plate.Release_Time, dbo.Plate.Wholesale_price, dbo.Company.Name, dbo.Plate.IDCompany_Creator, dbo.Plate.IDPlate", conn);
                            break;
                        case 2:
                            command = new SqlCommand("SELECT dbo.Plate.IDPlate as ID, dbo.Cover.Name AS Cover, dbo.Plate.N_In_Stock as Stock, dbo.Plate.Release_Time as Release, dbo.Plate.Wholesale_price as Price, " +
                                                      "dbo.Company.Name as Seller, (select dbo.Company.Name from dbo.Company where IDCompany = IDCompany_Creator) as Creator " +
                                                      "FROM dbo.Plate INNER JOIN dbo.Cover ON dbo.Plate.IDCover = dbo.Cover.IDCover inner join dbo.Company ON dbo.Plate.IDCompany_Seller = dbo.Company.IDCompany " +
                                                      "where IDCompany_Creator in (select IDCompany from dbo.Company group by IDCompany, Name having Name like '" + searchDiskTb.Text.Replace("'", "''") + "%') " +
                                                      "group by Cover.Name, dbo.Plate.N_In_Stock, dbo.Plate.Release_Time, dbo.Plate.Wholesale_price, dbo.Company.Name, dbo.Plate.IDCompany_Creator, dbo.Plate.IDPlate", conn);
                            break ;
                    }
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(searchTable);
                    disksInfo.ItemsSource = searchTable.DefaultView;
                    selectedDiskList.ItemsSource = searchTable.DefaultView;
                    if (searchTable.Rows.Count > 0)
                        selectedDiskList.SelectedIndex = 0;
                    else
                        selectedDiskList.SelectedIndex = -1;
                }
                else
                {
                    disksInfo.ItemsSource = dt.DefaultView;
                    selectedDiskList.ItemsSource = dt.DefaultView;
                }
                
            }
        }
        //Вибір шуканого елементу з пошуку
        private void musicSearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (musicSearchList.SelectedIndex != -1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (searchTable.Rows[musicSearchList.SelectedIndex][0].ToString() == dt.Rows[i][0].ToString() && searchTable.Rows[musicSearchList.SelectedIndex][1].ToString() == dt.Rows[i][1].ToString())
                        {
                            index = i;
                            break;
                        }
                    }
                    musicChanged();
                    musicSearchText.Text = "";
                    musicSearchList.SelectedIndex = -1;
                }
            }
            else if(coverShow.Visibility == Visibility.Visible)
            {
                if (coverSearchList.SelectedIndex != -1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (searchTable.Rows[coverSearchList.SelectedIndex][1].ToString() == dt.Rows[i][1].ToString())
                        {
                            index = i;
                            break;
                        }
                    }
                    musicChanged();
                    coverSearchText.Text = "";
                    coverSearchList.SelectedIndex = -1;
                }
            }
            else if (groupShow.Visibility == Visibility.Visible)
            {
                if (groupSearchList.SelectedIndex != -1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (searchTable.Rows[groupSearchList.SelectedIndex][1].ToString() == dt.Rows[i][1].ToString())
                        {
                            index = i;
                            break;
                        }
                    }
                    musicChanged();
                    groupSearchText.Text = "";
                    groupSearchList.SelectedIndex = -1;
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (musicianSearchList.SelectedIndex != -1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (searchTable.Rows[musicianSearchList.SelectedIndex][1].ToString() == dt.Rows[i][1].ToString())
                        {
                            index = i;
                            break;
                        }
                    }
                    musicChanged();
                    musicianSearchText.Text = "";
                    musicianSearchList.SelectedIndex = -1;
                }
            }
            else if(diskShow.Visibility == Visibility.Visible)
            {
                searchDiskTb.Text = "";
                musicChanged();
            }
        }
        #endregion


        #region Add
        //Відкриття вікна з додаванням
        private void addWindowOpen(object sender, MouseButtonEventArgs e)
        {
            if(musicShow.Visibility == Visibility.Visible)
            {
                musicAddWindow.Visibility = Visibility.Visible;
                musicAddList.SelectedIndex = 0;
                musicAddText.Text = "";
            }
            else if(coverShow.Visibility == Visibility.Visible)
            {
                coverAddWindow.Visibility = Visibility.Visible;
                coverAddText.Text = "";
            }
            else if(groupShow.Visibility == Visibility.Visible)
            {
                groupAddWindow.Visibility = Visibility.Visible;
                groupAddText.Text = "";
                groupAddList.SelectedIndex = 0;
            }
            else if(musicianShow.Visibility == Visibility.Visible)
            {
                musicianAddWindow.Visibility = Visibility.Visible;
                musicianAddText.Text = "";
                cmd = new SqlCommand("Select [Group].Name as Name, [Group].IDGroup from [Group]", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addTable);
                groupList.ItemsSource = addTable.DefaultView;
                groupList.SelectedIndex = 0;
            }
            else if (diskShow.Visibility == Visibility.Visible)
            {
                SqlCommand com = new SqlCommand("Select Company.Name as Name, Company.IDCompany from Company",conn);
                adapter = new SqlDataAdapter(com);
                DataTable data=new DataTable();
                adapter.Fill(data);
                companyCreatorList.ItemsSource = data.DefaultView;
                companySellerList.ItemsSource = data.DefaultView;
                com = new SqlCommand("Select Cover.Name as Name, Cover.IDCover from Cover", conn);
                adapter = new SqlDataAdapter(com);
                data = new DataTable();
                adapter.Fill(data);
                coverList.ItemsSource= data.DefaultView;
                diskAddWindow.Visibility = Visibility.Visible;
                coverList.SelectedIndex = 0;
                companyCreatorList.SelectedIndex = 0;
                companySellerList.SelectedIndex = 0;
                NInStockAddText.Text = "";
                WholesalePriceAddText.Text = "";
                releaseDate.SelectedDate = DateTime.Now;
            }
        }
        //Команда на додавання
        private void newMusicAdded(object sender, MouseButtonEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Visible) {
                if (musicAddText.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Music(Name, RecCondition) VALUES('" + musicAddText.Text.Replace("'", "''") + "', '" + musicAddList.Text + "')", conn);
                    cmd.ExecuteNonQuery();
                    dt.Rows.Clear();
                    adapter = new SqlDataAdapter(mainCmd);
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (musicAddText.Text == dt.Rows[i][0].ToString() && musicAddList.Text == dt.Rows[i][1].ToString())
                        {
                            index = i;
                            break;
                        }
                    }
                    musicChanged();
                    MessageBox.Show("The music named '" + musicAddText.Text + "' was successfuly added");
                    musicAddList.SelectedIndex = 0;
                    musicAddText.Text = "";
                    musicAddWindow.Visibility = Visibility.Hidden;
                }
            }
            else if (coverShow.Visibility == Visibility.Visible) {
                if (coverAddText.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Cover(Name) VALUES('" + coverAddText.Text.Replace("'", "''") + "')", conn);
                    cmd.ExecuteNonQuery();
                    dt.Rows.Clear();
                    adapter = new SqlDataAdapter(mainCmd);
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (coverAddText.Text == dt.Rows[i][0].ToString())
                        {
                            index = i;
                            break;
                        }
                    }
                    musicChanged();
                    MessageBox.Show("The cover named '" + coverAddText.Text + "' was successfuly added");
                    coverAddText.Text = "";
                    coverAddWindow.Visibility = Visibility.Hidden;
                }
            }
            else if (groupShow.Visibility == Visibility.Visible)
            {
                if (groupAddText.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO [Group](Name,Type) VALUES('" + groupAddText.Text.Replace("'", "''")+"','"+groupAddList.SelectedItem.ToString() + "')", conn);
                    cmd.ExecuteNonQuery();
                    dt.Rows.Clear();
                    adapter = new SqlDataAdapter(mainCmd);
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (groupAddText.Text == dt.Rows[i][0].ToString() && groupAddList.SelectedItem.ToString() == dt.Rows[i][2].ToString())
                        {
                            index = i;
                            break;
                        }
                    }
                    musicChanged();
                    MessageBox.Show("The group named '" + groupAddText.Text + "' was successfuly added");
                    groupAddText.Text = "";
                    groupAddList.SelectedIndex = 0;
                    groupAddWindow.Visibility = Visibility.Hidden;
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (musicianAddText.Text != "")
                {
                    SqlCommand cmd;
                    if (musicianAddText.Text.Split(' ').Length==1)
                        cmd = new SqlCommand("INSERT INTO Musician(Name,IDGroup, Surname) VALUES('" + musicianAddText.Text.Split(' ')[0].Replace("'", "''") + "'," + addTable.Rows[groupList.SelectedIndex][1] + ", '')", conn);
                    else
                        cmd = new SqlCommand("INSERT INTO Musician(Name,IDGroup,Surname) VALUES('" + musicianAddText.Text.Split(' ')[0].Replace("'", "''") + "'," + addTable.Rows[groupList.SelectedIndex][1] + ",'"+ musicianAddText.Text.Split(' ')[1].Replace("'", "''") + "')", conn);
                    cmd.ExecuteNonQuery();
                    dt.Rows.Clear();
                    adapter = new SqlDataAdapter(mainCmd);
                    adapter.Fill(dt);
                    if (musicianAddText.Text.Split(' ').Length == 1)
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (musicianAddText.Text == dt.Rows[i][0].ToString() && dt.Rows[i][2].ToString()=="" && addTable.Rows[groupList.SelectedIndex][1].ToString() == dt.Rows[i][3].ToString())
                            {
                                index = i;
                                break;
                            }
                        }
                    else
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                           if (musicianAddText.Text.Split(' ')[0] == dt.Rows[i][0].ToString()&& musicianAddText.Text.Split(' ')[1] == dt.Rows[i][2].ToString() && addTable.Rows[groupList.SelectedIndex][1].ToString() == dt.Rows[i][3].ToString())
                           {
                                index = i;
                                break;
                            }
                        }
                    musicChanged();
                    MessageBox.Show("The musician named '" + musicianAddText.Text + "' was successfuly added");
                    groupAddText.Text = "";
                    groupAddList.SelectedIndex = 0;
                    musicianAddWindow.Visibility = Visibility.Hidden;
                }
            }
            else if (diskShow.Visibility == Visibility.Visible)
            {
                if (NInStockAddText.Text != "" && WholesalePriceAddText.Text!="" && coverList.SelectedIndex != -1 && companyCreatorList.SelectedIndex!=-1&&companySellerList.SelectedIndex!=-1)
                {
                    SqlCommand cmd;
                    cmd = new SqlCommand("INSERT INTO Plate(IDCover,N_In_Stock,Release_Time, Wholesale_price, IDCompany_Seller, IDCompany_Creator) VALUES(" + ((DataRowView)coverList.SelectedItem).Row[1] + "," + NInStockAddText.Text + ",'" + ((DateTime)releaseDate.SelectedDate).ToString("yyyy-MM-dd") + "'," + WholesalePriceAddText.Text +","+ ((DataRowView)companySellerList.SelectedItem).Row[1] + "," + ((DataRowView)companyCreatorList.SelectedItem).Row[1] + ")", conn); ;
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    MessageBox.Show("The disk as successfuly added");
                    selectedDiskList.SelectedIndex = 0;
                    diskAddWindow.Visibility = Visibility.Hidden;
                }
            }
        }
        //Закриття вікна з додаванням
        private void addWindowClose(object sender, MouseButtonEventArgs e)
        {
            musicAddWindow.Visibility = Visibility.Hidden;
            coverAddWindow.Visibility = Visibility.Hidden;
            groupAddWindow.Visibility = Visibility.Hidden;
            musicianAddWindow.Visibility = Visibility.Hidden;
            diskAddWindow.Visibility= Visibility.Hidden;
        }
        #endregion


        #region Change
        //Відкриваємо вікно для змін
        private void musicChange(object sender, MouseButtonEventArgs e)
        {
            if(musicShow.Visibility == Visibility.Visible)
            {
                musicChangeWindow.Visibility = Visibility.Visible;
                musicChangeLabel.Text = dt.Rows[index][0].ToString();
            }
            else if(coverShow.Visibility == Visibility.Visible)
            {
                coverChangeWindow.Visibility = Visibility.Visible;
                coverChangeLabel.Text = dt.Rows[index][0].ToString();
            }
            else if(groupShow.Visibility == Visibility.Visible)
            {
                groupChangeWindow.Visibility = Visibility.Visible;
                groupChangeLabel.Text = dt.Rows[index][0].ToString();
            }
            else if(musicianShow.Visibility == Visibility.Visible)
            {
                musicianChangeWindow.Visibility = Visibility.Visible;
                musicianChangeLabel.Text = dt.Rows[index][0]+" "+ dt.Rows[index][2];
            }
            else if (diskShow.Visibility == Visibility.Visible)
            {
                if (selectedDiskList.SelectedIndex == -1)
                {
                    MessageBox.Show("Choose disk");
                    return;
                }
                diskChangeWindow.Visibility = Visibility.Visible;
            }
            musicChangeChanged();
        }
        //Оновлюємо відображення в вікні для змін
        private void musicChangeChanged()
        {
            if(musicShow.Visibility == Visibility.Visible)
            {
                deleteTable1 = new DataTable();
                deleteTable2 = new DataTable();
                addTable1 = new DataTable();
                addTable2 = new DataTable();
                SqlCommand cmd = new SqlCommand("select dbo.[Group].Name as Name, dbo.[Group].IDGroup  FROM dbo.[Group] INNER JOIN dbo.MusicGroup ON dbo.[Group].IDGroup = dbo.MusicGroup.IDGroup INNER JOIN dbo.Music ON dbo.MusicGroup.IDMusic = dbo.Music.IDMusic where dbo.Music.IDMusic = " + dt.Rows[index][2] + " GROUP BY dbo.[Group].Name, dbo.Music.IDMusic, dbo.[Group].IDGroup", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(deleteTable1);
                groupToDelete.ItemsSource = deleteTable1.DefaultView;
                cmd = new SqlCommand("select distinct dbo.[Group].Name as Name, dbo.[Group].IDGroup  FROM dbo.[Group] where IDGroup NOT IN (select dbo.[Group].IDGroup  FROM dbo.[Group] INNER JOIN dbo.MusicGroup ON dbo.[Group].IDGroup = dbo.MusicGroup.IDGroup INNER JOIN dbo.Music ON dbo.MusicGroup.IDMusic = dbo.Music.IDMusic where dbo.Music.IDMusic = " + dt.Rows[index][2] + " GROUP BY dbo.[Group].IDGroup, dbo.Music.IDMusic)", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addTable1);
                groupToAdd.ItemsSource = addTable1.DefaultView;
                cmd = new SqlCommand("select dbo.Genre.Name as Name, dbo.Genre.IDGenre FROM dbo.Genre INNER JOIN dbo.MusicGenre ON dbo.Genre.IDGenre = dbo.MusicGenre.IDGenre INNER JOIN dbo.Music ON dbo.MusicGenre.IDMusic = dbo.Music.IDMusic where dbo.Music.IDMusic = '" + dt.Rows[index][2] + "' GROUP BY dbo.Genre.Name, dbo.Music.IDMusic, dbo.Genre.IDGenre", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(deleteTable2);
                genreToDelete.ItemsSource = deleteTable2.DefaultView;
                cmd = new SqlCommand("select distinct dbo.Genre.Name as Name, dbo.Genre.IDGenre  FROM dbo.Genre where IDGenre NOT IN (select dbo.Genre.IDGenre FROM dbo.Genre INNER JOIN dbo.MusicGenre ON dbo.Genre.IDGenre = dbo.MusicGenre.IDGenre INNER JOIN dbo.Music ON dbo.MusicGenre.IDMusic = dbo.Music.IDMusic where dbo.Music.IDMusic = '" + dt.Rows[index][2] + "' GROUP BY dbo.Music.IDMusic, dbo.Genre.IDGenre)", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addTable2);
                genreToAdd.ItemsSource = addTable2.DefaultView;
            }
            else if(coverShow.Visibility == Visibility.Visible)
            {
                deleteTable1 = new DataTable();
                addTable1 = new DataTable();
                SqlCommand cmd = new SqlCommand("SELECT dbo.Music.Name+'('+dbo.Music.RecCondition+')' AS Name, dbo.Music.IDMusic FROM dbo.Music INNER JOIN dbo.Music_Cover ON dbo.Music.IDMusic = dbo.Music_Cover.IDMusic INNER JOIN dbo.Cover ON dbo.Music_Cover.IDCover = dbo.Cover.IDCover Where Cover.IDCover = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(deleteTable1);
                musicToDelete.ItemsSource = deleteTable1.DefaultView;
                cmd = new SqlCommand("select dbo.Music.Name+'('+dbo.Music.RecCondition+')' AS Name, dbo.Music.IDMusic  FROM dbo.Music where IDMusic NOT IN (SELECT dbo.Music.IDMusic FROM dbo.Music INNER JOIN dbo.Music_Cover ON dbo.Music.IDMusic = dbo.Music_Cover.IDMusic INNER JOIN dbo.Cover ON dbo.Music_Cover.IDCover = dbo.Cover.IDCover Where Cover.IDCover = " + dt.Rows[index][1]+")", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addTable1);
                musicToAdd.ItemsSource = addTable1.DefaultView;
                cmd = new SqlCommand("select Cover.Retail_Price, Cover.N_This_Year, Cover.N_Last_Year, Cover.N_All_Time from Cover Where IDCover = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                coverChangePrice.Text = table.Rows[0][0].ToString();
                coverChangeN0.Text = table.Rows[0][1].ToString();
                coverChangeN1.Text = table.Rows[0][2].ToString();
                coverChangeN2.Text = table.Rows[0][3].ToString();
            }
            else if (groupShow.Visibility == Visibility.Visible)
            {
                deleteTable1 = new DataTable();
                addTable1 = new DataTable();
                SqlCommand cmd = new SqlCommand("SELECT dbo.Musician.Name+' '+dbo.Musician.Surname As Name, dbo.Musician.IDMusician FROM dbo.[Group] INNER JOIN dbo.Musician ON dbo.[Group].IDGroup = dbo.Musician.IDGroup WHERE dbo.[Group].IDGroup = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(deleteTable1);
                musicianToDelete.ItemsSource = deleteTable1.DefaultView;
                cmd = new SqlCommand("select dbo.Musician.Name+' '+dbo.Musician.Surname As Name, dbo.Musician.IDMusician FROM dbo.Musician where IDMusician NOT IN (SELECT dbo.Musician.IDMusician FROM dbo.[Group] INNER JOIN dbo.Musician ON dbo.[Group].IDGroup = dbo.Musician.IDGroup WHERE dbo.[Group].IDGroup = " + dt.Rows[index][1] + ")", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addTable1);
                musicianToAdd.ItemsSource=addTable1.DefaultView;
                groupTypeList.SelectedItem = dt.Rows[index][2];
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                deleteTable1 = new DataTable();
                deleteTable2 = new DataTable();
                addTable1 = new DataTable();
                addTable2 = new DataTable();
                SqlCommand cmd = new SqlCommand("SELECT dbo.Instrument.Name as Name, dbo.Instrument.IDInstrument FROM dbo.Musician INNER JOIN dbo.MusicianInstrument ON dbo.Musician.IDMusician = dbo.MusicianInstrument.IDMusician INNER JOIN dbo.Instrument ON dbo.MusicianInstrument.IDInstrument = dbo.Instrument.IDInstrument WHERE dbo.Musician.IDMusician = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(deleteTable1);
                instrumentToDelete.ItemsSource = deleteTable1.DefaultView;
                cmd = new SqlCommand("select distinct dbo.Instrument.Name as Name, dbo.Instrument.IDInstrument  FROM dbo.Instrument where IDInstrument NOT IN (SELECT dbo.Instrument.IDInstrument FROM dbo.Musician INNER JOIN dbo.MusicianInstrument ON dbo.Musician.IDMusician = dbo.MusicianInstrument.IDMusician INNER JOIN dbo.Instrument ON dbo.MusicianInstrument.IDInstrument = dbo.Instrument.IDInstrument WHERE dbo.Musician.IDMusician = " + dt.Rows[index][1]+")", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addTable1);
                instrumentToAdd.ItemsSource = addTable1.DefaultView;
                cmd = new SqlCommand("SELECT dbo.Occupation.Name as Name, dbo.Occupation.IDOccupation FROM dbo.Musician INNER JOIN dbo.MusicianOccupation ON dbo.Musician.IDMusician = dbo.MusicianOccupation.IDMusician INNER JOIN dbo.Occupation ON dbo.MusicianOccupation.IDOccupation = dbo.Occupation.IDOccupation WHERE dbo.Musician.IDMusician = " + dt.Rows[index][1], conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(deleteTable2);
                occupationToDelete.ItemsSource = deleteTable2.DefaultView;
                cmd = new SqlCommand("select distinct dbo.Occupation.Name, dbo.Occupation.IDOccupation  FROM dbo.Occupation where IDOccupation NOT IN (SELECT dbo.Occupation.IDOccupation AS Occupation FROM dbo.Musician INNER JOIN dbo.MusicianOccupation ON dbo.Musician.IDMusician = dbo.MusicianOccupation.IDMusician INNER JOIN dbo.Occupation ON dbo.MusicianOccupation.IDOccupation = dbo.Occupation.IDOccupation WHERE dbo.Musician.IDMusician = " + dt.Rows[index][1]+")", conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(addTable2);
                occupationToAdd.ItemsSource = addTable2.DefaultView;
            }
            else if (diskShow.Visibility == Visibility.Visible)
            {
                SqlCommand com = new SqlCommand("Select Company.Name as Name, Company.IDCompany from Company", conn);
                adapter = new SqlDataAdapter(com);
                DataTable data = new DataTable();
                adapter.Fill(data);
                creatorToAdd.ItemsSource = data.DefaultView;
                sellerToAdd.ItemsSource = data.DefaultView;
                com = new SqlCommand("Select Cover.Name as Name, Cover.IDCover from Cover", conn);
                adapter = new SqlDataAdapter(com);
                data = new DataTable();
                adapter.Fill(data);
                coverDiskToAdd.ItemsSource = data.DefaultView;
                data = new DataTable();
                com = new SqlCommand("select Cover.Name as Name, Cover.IDCover from Cover inner join Plate ON Plate.IDCover=Cover.IDCover where IDPlate="+selectedDiskList.Text, conn);
                adapter = new SqlDataAdapter(com);
                adapter.Fill(data);
                DataRowView n;
                for (int i = 0; i < coverDiskToAdd.Items.Count; i++)
                {
                    n = (DataRowView)coverDiskToAdd.Items[i];
                    if (n.Row[1].ToString() == data.Rows[0][1].ToString())
                    {
                        coverDiskToAdd.SelectedIndex = i;
                        break;
                    }
                }
                data = new DataTable();
                com = new SqlCommand("select Company.Name as Name, Company.IDCompany from Company inner join Plate ON Plate.IDCompany_Creator=Company.IDCompany where IDPlate=" + selectedDiskList.Text, conn);
                adapter = new SqlDataAdapter(com);
                adapter.Fill(data);
                for (int i = 0; i < creatorToAdd.Items.Count; i++)
                {
                    n = (DataRowView)creatorToAdd.Items[i];
                    if (n.Row[1].ToString() == data.Rows[0][1].ToString())
                    {
                        creatorToAdd.SelectedIndex = i;
                        break;
                    }
                }
                data = new DataTable();
                com = new SqlCommand("select Company.Name as Name, Company.IDCompany from Company inner join Plate ON Plate.IDCompany_Seller=Company.IDCompany where IDPlate=" + selectedDiskList.Text, conn);
                adapter = new SqlDataAdapter(com);
                adapter.Fill(data);
                for(int i =0;i<sellerToAdd.Items.Count;i++)
                {
                    n = (DataRowView)sellerToAdd.Items[i];
                    if (n.Row[1].ToString() == data.Rows[0][1].ToString())
                    {
                        sellerToAdd.SelectedIndex = i;
                        break;
                    }
                }
                wholesalePriceChange.Text = ((DataRowView)selectedDiskList.SelectedItem).Row[4].ToString();
                NInStockChange.Text = ((DataRowView)selectedDiskList.SelectedItem).Row[2].ToString();
                dateChange.SelectedDate =(DateTime)((DataRowView)selectedDiskList.SelectedItem).Row[3];
            }
        }
        //Змінюємо назву основного елементу
        private void nameChanged(object sender, TextChangedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand("", conn);
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (musicChangeLabel.Text != "")
                {
                    cmd = new SqlCommand("UPDATE Music SET Name = '" + musicChangeLabel.Text.Replace("'", "''") + "' where IDMusic = " + dt.Rows[index][2].ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (coverShow.Visibility == Visibility.Visible)
            {
                if (coverChangeLabel.Text != "")
                {
                    cmd = new SqlCommand("UPDATE Cover SET Name = '" + coverChangeLabel.Text.Replace("'", "''") + "' where IDCover = " + dt.Rows[index][1].ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else if(groupShow.Visibility == Visibility.Visible)
            {
                if (groupChangeLabel.Text != "")
                {
                    cmd = new SqlCommand("UPDATE [Group] SET Name = '" + groupChangeLabel.Text.Replace("'", "''") + "' where IDGroup = " + dt.Rows[index][1].ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (musicianChangeLabel.Text != "")
                {
                    if(musicianChangeLabel.Text.Split(' ').Length==1)
                        cmd = new SqlCommand("UPDATE Musician SET Name = '" + musicianChangeLabel.Text.Replace("'", "''") + "', Surname = null where IDMusician = " + dt.Rows[index][1].ToString(), conn);
                    else
                        cmd = new SqlCommand("UPDATE Musician SET Name = '" + musicianChangeLabel.Text.Split(' ')[0].Replace("'", "''") + "', Surname='"+ musicianChangeLabel.Text.Split(' ')[1].Replace("'", "''") + "' where IDMusician = " + dt.Rows[index][1].ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            musicChanged();
        }
        //Додаємо перший елемент 
        private void addItem1(object sender, MouseButtonEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (groupToAdd.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO MusicGroup(IDMusic, IDGroup) VALUES(" + dt.Rows[index][2] + ", " + addTable1.Rows[groupToAdd.SelectedIndex][1] + ")", conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if (coverShow.Visibility == Visibility.Visible)
            {
                if (musicToAdd.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Music_Cover(IDMusic, IDCover) VALUES(" + addTable1.Rows[musicToAdd.SelectedIndex][1] + ", " + dt.Rows[index][1] + ")", conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if(groupShow.Visibility == Visibility.Visible)
            {
                if (musicianToAdd.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("Update Musician Set IDGroup = "+ dt.Rows[index][1] + " Where IDMusician = " + addTable1.Rows[musicianToAdd.SelectedIndex][1], conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (instrumentToAdd.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO MusicianInstrument(IDInstrument, IDMusician) VALUES(" + addTable1.Rows[instrumentToAdd.SelectedIndex][1] + ", " + dt.Rows[index][1] + ")", conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
        }
        //Видаляємо перший елемент 
        private void DeleteItem1(object sender, MouseButtonEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (groupToDelete.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("Delete from MusicGroup where IDMusic = " + dt.Rows[index][2] + " and IDGroup = " + deleteTable1.Rows[groupToDelete.SelectedIndex][1], conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if (coverShow.Visibility == Visibility.Visible)
            {
                if (musicToDelete.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("Delete from Music_Cover where IDCover = " + dt.Rows[index][1] + " and IDMusic = " + deleteTable1.Rows[musicToDelete.SelectedIndex][1], conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if(groupShow.Visibility == Visibility.Visible)
            {
                if (musicianToDelete.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("Update Musician Set IDGroup = null Where IDMusician = " + deleteTable1.Rows[musicianToDelete.SelectedIndex][1], conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (instrumentToDelete.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("Delete from MusicianInstrument where IDMusician = " + dt.Rows[index][1] + " and IDInstrument = " + deleteTable1.Rows[instrumentToDelete.SelectedIndex][1], conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
        }
        //Додаємо другий елемент 
        private void addItem2(object sender, MouseButtonEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (genreToAdd.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO MusicGenre(IDMusic, IDGenre) VALUES(" + dt.Rows[index][2] + ", " + addTable2.Rows[genreToAdd.SelectedIndex][1] + ")", conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (occupationToAdd.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO MusicianOccupation(IDOccupation, IDMusician) VALUES(" + addTable2.Rows[occupationToAdd.SelectedIndex][1] + ", " + dt.Rows[index][1] + ")", conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
        }
        //Видаляємо другий елемент
        private void DeleteItem2(object sender, MouseButtonEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Visible)
            {
                if (genreToDelete.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("Delete from MusicGenre where IDMusic = " + dt.Rows[index][2] + " and IDGenre =" + deleteTable2.Rows[genreToDelete.SelectedIndex][1] + "", conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                if (occupationToDelete.SelectedIndex != -1)
                {
                    SqlCommand cmd = new SqlCommand("Delete from MusicianOccupation where IDMusician = " + dt.Rows[index][1] + " and IDOccupation = " + deleteTable2.Rows[occupationToDelete.SelectedIndex][1], conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    musicChangeChanged();
                }
            }
        }
        //Текстове поле 1
        private void textOne(object sender, TextChangedEventArgs e)
        {
            if (coverShow.Visibility == Visibility.Visible)
            {
                if (coverChangePrice.Text != "")
                {
                    cmd = new SqlCommand("UPDATE Cover SET Retail_Price = " + coverChangePrice.Text + " where IDCover = " + dt.Rows[index][1].ToString(), conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                }
                
            }
            else if(diskShow.Visibility == Visibility.Visible)
            {
                if (wholesalePriceChange.Text != "")
                {
                    int Ind = selectedDiskList.SelectedIndex;
                    cmd = new SqlCommand("UPDATE Plate SET Wholesale_price = " + wholesalePriceChange.Text + " where IDPlate = "+ selectedDiskList.Text, conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    selectedDiskList.SelectedIndex = Ind;
                }

            }
        }
        //Текстове поле 2
        private void textTwo(object sender, TextChangedEventArgs e)
        {
            if (coverShow.Visibility == Visibility.Visible)
            {
                if (coverChangeN0.Text != "")
                {
                    cmd = new SqlCommand("UPDATE Cover SET N_This_Year = " + coverChangeN0.Text + " where IDCover = " + dt.Rows[index][1].ToString(), conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                }
            }
            else if (diskShow.Visibility == Visibility.Visible)
            {
                if (NInStockChange.Text != "")
                {
                    int Ind = selectedDiskList.SelectedIndex;
                    cmd = new SqlCommand("UPDATE Plate SET N_In_Stock = " + NInStockChange.Text + " where IDPlate = " + selectedDiskList.Text, conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                    selectedDiskList.SelectedIndex = Ind;
                }

            }
        }
        //Текстове поле 3
        private void textThree(object sender, TextChangedEventArgs e)
        {
            if (coverShow.Visibility == Visibility.Visible)
            {
                if (coverChangeN1.Text != "")
                {
                    cmd = new SqlCommand("UPDATE Cover SET N_Last_Year = " + coverChangeN1.Text + " where IDCover = " + dt.Rows[index][1].ToString(), conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                }
            }
        }
        //Текстове поле 4
        private void textFour(object sender, TextChangedEventArgs e)
        {
            if (coverShow.Visibility == Visibility.Visible)
            {
                if (coverChangeN2.Text != "")
                {
                    cmd = new SqlCommand("UPDATE Cover SET N_All_Time = " + coverChangeN2.Text + " where IDCover = " + dt.Rows[index][1].ToString(), conn);
                    cmd.ExecuteNonQuery();
                    musicChanged();
                }
            }
        }
        //Зміна комбобокса
        private void typeChanged(object sender, SelectionChangedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Update [Group] Set Type = '" + groupTypeList.SelectedItem.ToString() + "' Where IDGroup = " + dt.Rows[index][1], conn);
            cmd.ExecuteNonQuery();
            musicChanged();
            musicChangeChanged();
        }
        //Зміна дати
        private void dateChange_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dateChange.SelectedDate != null)
            {
                int Ind = selectedDiskList.SelectedIndex;
                cmd = new SqlCommand("UPDATE Plate SET Release_Time ='" + dateChange.SelectedDate.Value.ToString("yyyy-MM-dd") + "' where IDPlate = " + selectedDiskList.Text, conn);
                cmd.ExecuteNonQuery();
                musicChanged();
                selectedDiskList.SelectedIndex = Ind;
            }
        }
        //Закриваємо вікно для змін
        private void changeWindowClose(object sender, MouseButtonEventArgs e)
        {
            musicChangeWindow.Visibility = Visibility.Hidden;
            coverChangeWindow.Visibility = Visibility.Hidden;
            groupChangeWindow.Visibility = Visibility.Hidden;
            musicianChangeWindow.Visibility= Visibility.Hidden;
            diskChangeWindow.Visibility= Visibility.Hidden;
        }
        #endregion


        //Видаляємо основний елемент
        private void deleteMusic(object sender, MouseButtonEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Music named '" + dt.Rows[index][0] + "' was succsessfuly deleted");
                SqlCommand cmd = new SqlCommand("Delete from Music where IDMusic = " + dt.Rows[index][2], conn);
                cmd.ExecuteNonQuery();
                dt = new DataTable();
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                index = (index - 1 + dt.Rows.Count) % dt.Rows.Count;
                musicChanged();
            }
            else if (coverShow.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Cover named '" + dt.Rows[index][0] + "' was succsessfuly deleted");
                SqlCommand cmd = new SqlCommand("Delete from Cover where IDCover = " + dt.Rows[index][1], conn);
                cmd.ExecuteNonQuery();
                dt = new DataTable();
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                index = (index - 1 + dt.Rows.Count) % dt.Rows.Count;
                musicChanged();
            }
            else if (groupShow.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Group named '" + dt.Rows[index][0] + "' was succsessfuly deleted");
                SqlCommand cmd = new SqlCommand("Delete from [Group] where IDGroup = " + dt.Rows[index][1], conn);
                cmd.ExecuteNonQuery();
                dt = new DataTable();
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                index = (index - 1 + dt.Rows.Count) % dt.Rows.Count;
                musicChanged();
            }
            else if (musicianShow.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Musician named '" + dt.Rows[index][0] + "' was succsessfuly deleted");
                SqlCommand cmd = new SqlCommand("Delete from Musician where IDMusician = " + dt.Rows[index][1], conn);
                cmd.ExecuteNonQuery();
                dt = new DataTable();
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                index = (index - 1 + dt.Rows.Count) % dt.Rows.Count;
                musicChanged();
            }
            else if(diskShow.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Disk with ID '" + selectedDiskList.Text + "' was succsessfuly deleted");
                SqlCommand cmd = new SqlCommand("Delete from Plate where IDPlate = " + selectedDiskList.Text, conn);
                cmd.ExecuteNonQuery();
                dt = new DataTable();
                adapter = new SqlDataAdapter(mainCmd);
                adapter.Fill(dt);
                musicChanged();
            }
        }


        #region ShowTabs
        //Відкриваємо іншу сторінку
        private void showTab(Grid target,int IDtab)
        {
            int i = 0;
            string[] images = new string[] { "icons8-музыка-30.png",
                                             "icons8-плейлист-lounge-30.png",
                                             "icons8-хор-30.png",
                                             "icons8-микрофон-30.png",
                                             "icons8-пластинка-30.png",
                                             "icons8-настройки-30.png"};
            foreach (Grid tab in menuShowTabs.Children)
            {
                foreach(Grid item in tab.Children.OfType<Grid>())
                {
                    item.Visibility=Visibility.Hidden;
                }
                tab.Visibility=Visibility.Hidden;
            }
            target.Visibility=Visibility.Visible;
            foreach (Grid g in menuGrid.Children.OfType<Grid>())
            {
                if (i != IDtab)
                {
                    g.Cursor = Cursors.Hand;
                    foreach (Label label in g.Children.OfType<Label>())
                    {
                        label.Foreground = new SolidColorBrush(Color.FromRgb(167, 166, 166));
                    }
                    foreach (Image img in g.Children.OfType<Image>())
                    {
                        img.Source = new BitmapImage(new Uri(images[i], UriKind.Relative));
                        i++;
                    }
                }
                else
                {
                    g.Cursor = Cursors.Arrow;
                    foreach (Label label in g.Children.OfType<Label>())
                    {
                        label.Foreground = new SolidColorBrush(Colors.White);
                    }
                    foreach (Image img in g.Children.OfType<Image>())
                    {
                        img.Source = new BitmapImage(new Uri(images[i].Replace(".png"," (1).png"), UriKind.Relative));
                        i++;
                    }
                }
            }
        }
        //Відкриваємо вікно з музикою
        private void musicShowing(object sender, MouseButtonEventArgs e)
        {
            if (musicShow.Visibility == Visibility.Hidden)
            {
                index = 0;
                showTab(musicShow, 0);
                musicChanged();
            }
        }
        //Відкриваємо вікно з плейлістами
        private void coverShowing(object sender, MouseButtonEventArgs e)
        {
            if (coverShow.Visibility == Visibility.Hidden)
            {
                index = 0;
                showTab(coverShow,1);
                musicChanged();
            }
        }
        //Відкриваємо вікно з групами
        private void groupShowing(object sender, MouseButtonEventArgs e)
        {
            if (groupShow.Visibility == Visibility.Hidden)
            {
                index = 0;
                showTab(groupShow, 2);
                musicChanged();
            }
        }
        //Відкриваємо вікно з музикантами
        private void musicianShowing(object sender, MouseButtonEventArgs e)
        {
            if (musicianShow.Visibility == Visibility.Hidden)
            {
                index = 0;
                showTab(musicianShow, 3);
                musicChanged();
            }
        }
        //Відкриваємо вікно з дисками
        private void diskShowing(object sender, MouseButtonEventArgs e)
        {
            if (diskShow.Visibility == Visibility.Hidden)
            {
                index = 0;
                showTab(diskShow, 4);
                musicChanged();
            }
        }
        #endregion


        //Виходимо з застосунку
        private void exit(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        
    }
}
