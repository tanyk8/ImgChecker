

        private void updateRejectFolderList()
        {
            string[] folder = Directory.GetDirectories(rejectPath).Select(System.IO.Path.GetFileName).ToArray();


            if (rejectFolderList.Count > 0)
            {
                rejectFolderList.Clear();
            }


            foreach (string foldername in folder)
            {
                string percentage = "0.00";
                if (countSelectedCategory(foldername) != 0)
                {
                    percentage = (countSelectedCategory(foldername) / (double)rejectcount * 100).ToString("F");
                }


                rejectFolderList.Add(new RejectFolder() { rejectFolderName = System.IO.Path.GetFileName(foldername), rejectNum = "(" + countSelectedCategory(foldername) + " img) " + "(" + percentage + "%)" });
            }



            this.rejectListBox.ItemsSource = rejectFolderList;
            rejectListBox.Items.Refresh();
            this.rejectOverviewListBox.ItemsSource = rejectFolderList;
            rejectOverviewListBox.Items.Refresh();
            this.summaryListBox.ItemsSource = rejectFolderList;
            summaryListBox.Items.Refresh();
            this.rejectManageBox.ItemsSource = rejectFolderList;
            rejectManageBox.Items.Refresh();

            if (rejectFolderList.Count == 0 && currTabName == "Reject" || rejectFolderList.Count == 0 && RejectBox.Visibility.ToString() == "Visible")
            {
                MessageBox.Show("No reject category found! Please add a reject category!");
            }
        }

        private void selectReject_doubleclick(object sender, RoutedEventArgs e)
        {
            if (rejectListBox.SelectedItem != null)
            {
                RejectFolder temp = (RejectFolder)rejectListBox.SelectedItem;
                string selectedDFolder = temp.getRejectFolderName();
                // string[] files = Directory.GetFiles(uploadPath);

                img = (Image)this.FindName("imgi");

                if (imageFiles.Count == 0)
                {
                    MessageBox.Show("No image found in the upload folder!");
                    pfBtn = (Button)this.FindName("btnPass");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnReject");
                    pfBtn.IsEnabled = false;
                    return;
                }

                int index = imageFiles.FindIndex(x => x.Contains(System.IO.Path.GetFileName(img.Source.ToString())));

                string sourceFile = uploadPath + "\\" + imageFiles.ElementAt(index);
                string destinationFile = rejectPath + "\\" + selectedDFolder + "\\" + imageFiles.ElementAt(index);

                if (File.Exists(destinationFile) || checkReject(selectedDFolder, imageFiles.ElementAt(index)))
                {
                    MessageBoxResult mbresult;
                    mbresult = MessageBox.Show("Duplicate file name found at target location! Would you like to rename the image file name?\n" + destinationFile, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (mbresult == MessageBoxResult.Yes)
                    {
                        string extpart = Path.GetExtension(destinationFile);
                        string namepart = Path.GetFileNameWithoutExtension(destinationFile);

                        bool repeat = true;
                        int incNum = 1;

                        do
                        {
                            if (File.Exists(rejectPath + "\\" + selectedDFolder + "\\" + namepart + " (1)" + extpart))
                            {
                                incNum++;
                            }
                            else
                            {
                                repeat = false;
                                destinationFile = rejectPath + "\\" + selectedDFolder + "\\" + namepart + " (1)" + extpart;
                            }
                        } while (repeat);

                    }
                    else
                    {
                        return;
                    }
                }

                if (File.Exists(sourceFile))
                {
                    fImageFiles.Add(new RejectFile(selectedDFolder, Path.GetFileName(destinationFile)));
                    imageFiles.RemoveAt(index);
                    for (int count = 0; count < 10; count++)
                    {
                        if (count < imageFiles.Count)
                        {
                            img = (Image)this.FindName("img" + (count + 1));
                            img.Source = setImgSource(uploadPath + "\\" + imageFiles.ElementAt(count), "sub");

                            if (count == 0)
                            {
                                img = (Image)this.FindName("imgi");
                                img.Source = setImgSource(uploadPath + "\\" + imageFiles.ElementAt(count), "main");
                            }
                        }

                        if (imageFiles.Count < 10 && count >= imageFiles.Count)
                        {
                            img = (Image)this.FindName("img" + (imageFiles.Count + 1).ToString());
                            img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                        }
                    }
                }

                try
                {
                    System.IO.File.Move(sourceFile, destinationFile);
                }
                catch
                {
                    MessageBox.Show("Error! The file is either missing or corrupted!");
                    return;
                }

                counter = (Label)this.FindName("rejectCount");
                rejectcount++;
                numProgress++;
                counter.Content = "Total Reject Count: " + rejectcount;
                counter = (Label)this.FindName("progressCount");
                counter.Content = "Overall progress: " + numProgress + "/" + totalNum;

                if (currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
                {
                    if (selectedDFolder == currRejectFolder)
                    {
                        //updateReject("load", "content");

                        currRejectPage = 1;
                        counter = (Label)this.FindName("rejectPage");
                        counter.Content = currRejectPage;
                        changeRejectPage(currRejectPage, "content", "");

                        pfBtn = (Button)this.FindName("btnRejectPrev");
                        pfBtn.IsEnabled = false;
                        pfBtn = (Button)this.FindName("btnRejectFirst");
                        pfBtn.IsEnabled = false;


                        if (countSelectedCategory(temp.getRejectFolderName()) > 10)
                        {
                            pfBtn = (Button)this.FindName("btnRejectNext");
                            pfBtn.IsEnabled = true;
                            pfBtn = (Button)this.FindName("btnRejectLast");
                            pfBtn.IsEnabled = true;
                        }
                        else
                        {
                            pfBtn = (Button)this.FindName("btnRejectNext");
                            pfBtn.IsEnabled = false;
                            pfBtn = (Button)this.FindName("btnRejectLast");
                            pfBtn.IsEnabled = false;
                        }
                    }

                }


                if (imageFiles.Count == 0)
                {
                    img = (Image)this.FindName("img1");
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                    img = (Image)this.FindName("imgi");
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));

                    MessageBox.Show("All image uploaded have been checked!");

                    pfBtn = (Button)this.FindName("btnPass");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnReject");
                    pfBtn.IsEnabled = false;
                    setButtonStatus("btnDeleteImg", false);

                }
                else
                {
                    currAllPage = 1;

                    if (imageFiles.Count < 11)
                    {
                        pfBtn = (Button)this.FindName("btnAllNext");
                        pfBtn.IsEnabled = false;
                        pfBtn = (Button)this.FindName("btnAllLast");
                        pfBtn.IsEnabled = false;
                    }
                    else
                    {
                        pfBtn = (Button)this.FindName("btnAllNext");
                        pfBtn.IsEnabled = true;
                        pfBtn = (Button)this.FindName("btnAllLast");
                        pfBtn.IsEnabled = true;
                    }

                    pfBtn = (Button)this.FindName("btnAllPrev");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnAllFirst");
                    pfBtn.IsEnabled = false;

                    counter = (Label)this.FindName("allPage");
                    counter.Content = currAllPage;
                }

                if (pImageFiles.Count > 10)
                {
                    pfBtn = (Button)this.FindName("btnPassNext");
                    pfBtn.IsEnabled = true;
                    pfBtn = (Button)this.FindName("btnPassLast");
                    pfBtn.IsEnabled = true;
                }
                updateRejectFolderList();

                if (System.IO.Path.GetFileName(imgi.Source.ToString()) == "imagemissing.png")
                {
                    //disable and hide revert, pass and reject
                    pfBtn = (Button)this.FindName("btnPass");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnReject");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnRevert");
                    pfBtn.IsEnabled = false;
                    setButtonStatus("btnDeleteImg", false);
                    btnPass.Visibility = System.Windows.Visibility.Collapsed;
                    btnReject.Visibility = System.Windows.Visibility.Collapsed;
                    btnRevert.Visibility = System.Windows.Visibility.Collapsed;
                    //show button
                    pfBtn = (Button)this.FindName("btnInfo");
                    pfBtn.IsEnabled = true;
                    btnInfo.Visibility = System.Windows.Visibility.Visible;
                    activemissingfile = "uploaded\\" + imageFiles.ElementAt(0);
                    checkActive();
                    img1.Opacity = 1;
                }
                else
                {
                    checkActive();
                }


                panzoom = (PanAndZoom)this.FindName("border");
                panzoom.Reset();
                RejectBox.Visibility = System.Windows.Visibility.Collapsed;
                saveFile();
            }
        }

        private void cancelRejectList_Click(object sender, RoutedEventArgs e)
        {
            RejectBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void rejectTabFolder_doubleclick(object sender, MouseEventArgs e)
        {

            rejectFolderOverview.Visibility = System.Windows.Visibility.Collapsed;
            rejectFolderContent.Visibility = System.Windows.Visibility.Visible;
            if (rejectOverviewListBox.SelectedItem != null)
            {
                RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
                string folname = temp.getRejectFolderName();
                currRejectFolder = folname;
                Button openrejectbtn = (Button)this.FindName("btnOpenDefFol");
                openrejectbtn.Content = "Open " + "\"" + folname + "\"";

                if (countSelectedCategory(folname) < 11)
                {
                    pfBtn = (Button)this.FindName("btnRejectNext");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnRejectLast");
                    pfBtn.IsEnabled = false;
                }
                else
                {
                    pfBtn = (Button)this.FindName("btnRejectNext");
                    pfBtn.IsEnabled = true;
                    pfBtn = (Button)this.FindName("btnRejectLast");
                    pfBtn.IsEnabled = true;
                }

                //updateReject("load");

                currRejectPage = 1;
                counter = (Label)this.FindName("rejectPage");
                counter.Content = currRejectPage;
                changeRejectPage(currRejectPage, "overview", "");

                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = false;


                if (countSelectedCategory(folname) > 10)
                {
                    pfBtn = (Button)this.FindName("btnRejectNext");
                    pfBtn.IsEnabled = true;
                    pfBtn = (Button)this.FindName("btnRejectLast");
                    pfBtn.IsEnabled = true;
                }
                else
                {
                    pfBtn = (Button)this.FindName("btnRejectNext");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnRejectLast");
                    pfBtn.IsEnabled = false;
                }
            }
        }

        private void btnReturnOverview_Click(object sender, RoutedEventArgs e)
        {
            rejectFolderContent.Visibility = System.Windows.Visibility.Collapsed;
            rejectFolderOverview.Visibility = System.Windows.Visibility.Visible;
            currRejectFolder = "";
            updateReject("unload", "overview");
        }

        private void btnOpenRejectFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", @"" + rejectPath + "");
        }

        private void btnOpenRejectCategory_Click(object sender, RoutedEventArgs e)
        {
            if (rejectOverviewListBox.SelectedItem != null)
            {
                RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
                string foldernameselected = temp.getRejectFolderName();
                Process.Start("explorer.exe", @"" + rejectPath + "\\" + foldernameselected);
            }
        }

        private void updateReject(string operType, string caseOption)
        {


            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected;



            if (rejectOverviewListBox.SelectedItem != null || rejectListBox.SelectedItem != null)
            {
                if (caseOption == "overview")
                {
                    temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
                    foldernameselected = temp.getRejectFolderName();
                }
                else if (caseOption == "content")
                {
                    temp = (RejectFolder)rejectListBox.SelectedItem;
                    foldernameselected = temp.getRejectFolderName();
                }
            }

            //string fullpath;
            //if (operType == "load")
            //{
            //    int num = 1;

            //    for (int x = fImageFiles.Count - 1; x >= 0; x--)
            //    {
            //        fullpath = rejectPath + "\\" + fImageFiles.ElementAt(x).getRejectFolderName() + "\\" + fImageFiles.ElementAt(x).getRejectFileName();
            //        if (Regex.IsMatch(System.IO.Path.GetFileName(fullpath), @"\.jpg$|\.png$|\.gif$|\.JPG$"))
            //        {
            //            if (num < 11)
            //            {

            //                if (fImageFiles.ElementAt(x).getRejectFolderName() == temp.getRejectFolderName())
            //                {
            //                    img = (Image)this.FindName("Fimg" + num);
            //                    img.Source = setImgSource(fullpath, "sub");
            //                    num++;
            //                }

            //            }
            //            else
            //            {
            //                return;
            //            }

            //        }
            //    }
            //}
            //else if
            if (operType == "unload")
            {
                for (int i = 1; i < 11; i++)
                {
                    img = (Image)this.FindName("Fimg" + i);
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                }
            }

        }
        //=================pass page navigation====================
        private void btnPassPrev_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage--;
            counter.Content = currPassPage;

            if (currPassPage != 1 || pImageFiles.Count > 10)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = true;
            }

            if (currPassPage == 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = false;
            }

            changePassPage(currPassPage);
        }

        private void btnPassNext_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage++;
            counter.Content = currPassPage;

            if (currPassPage != 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = true;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = false;
            }

            int finalpage = (pImageFiles.Count - 1) / 10 + 1;

            if (currPassPage == finalpage)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = false;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = true;
            }

            changePassPage(currPassPage);
        }

        private void changePassPage(int currPage)
        {
            int num = 1;
            for (int count = pImageFiles.Count - ((currPage - 1) * 10 + 1); count >= pImageFiles.Count - (currPage * 10); count--)
            {
                img = (Image)this.FindName("Pimg" + num);

                if (count >= 0)
                {
                    img.Source = setImgSource(passPath + "\\" + pImageFiles.ElementAt(count), "sub");
                }
                else
                {
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                }
                num++;
            }
            checkActive();
        }

        private void btnPassFirst_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage = 1;
            counter.Content = currPassPage;




            if (currPassPage != 1 || pImageFiles.Count > 10)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = true;
            }

            if (currPassPage == 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = false;
            }


            changePassPage(currPassPage);
        }

        private void btnPassLast_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage = (pImageFiles.Count - 1) / 10 + 1;
            counter.Content = currPassPage;

            if (currPassPage != 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = true;
            }

            if (pImageFiles.Count < currPassPage * 10 + 1)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = false;
            }

            changePassPage(currPassPage);
        }

        //-----------------end pass page navigation-------------------

        //=================reject page navigation====================
        private void btnRejectPrev_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage--;
            counter.Content = currRejectPage;

            if (currRejectPage != 1 || countSelectedCategory(foldernameselected) > 10)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = true;
            }

            if (currRejectPage == 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = false;
            }

            changeRejectPage(currRejectPage, "overview", "");
        }

        private void btnRejectNext_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage++;
            counter.Content = currRejectPage;

            if (currRejectPage != 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = true;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = false;
            }

            int finalpage = (countSelectedCategory(foldernameselected) - 1) / 10 + 1;

            if (currRejectPage == finalpage)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = false;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = true;
            }

            changeRejectPage(currRejectPage, "overview", "");
        }

        private void changeRejectPage(int currPage, string caseOption, string specialcase)
        {
            RejectFolder temp;
            string foldernameselected = "";
            if (rejectOverviewListBox.SelectedItem != null || rejectListBox.SelectedItem != null)
            {
                if (caseOption == "overview")
                {
                    temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
                    foldernameselected = temp.getRejectFolderName();
                }
                else if (caseOption == "content")
                {
                    temp = (RejectFolder)rejectListBox.SelectedItem;
                    foldernameselected = temp.getRejectFolderName();
                }
            }

            if (caseOption == "special")
            {
                foldernameselected = specialcase;
            }

            if (caseOption == "special1")
            {
                foldernameselected = currRejectFolder;
            }

            int num = 1;

            List<RejectFile> tempList = new List<RejectFile>();

            for (int i = 0; i < fImageFiles.Count; i++)
            {
                if (fImageFiles.ElementAt(i).getRejectFolderName() == foldernameselected)
                {
                    tempList.Add(new RejectFile(foldernameselected, fImageFiles.ElementAt(i).getRejectFileName()));
                }
            }


            for (int count = countSelectedCategory(foldernameselected) - 1; count >= countSelectedCategory(foldernameselected) - (currRejectPage * 10); count--)
            {

                img = (Image)this.FindName("Fimg" + num);

                if (count >= 0)
                {
                    string fullpath = rejectPath + "\\" + tempList.ElementAt(count).getRejectFolderName() + "\\" + tempList.ElementAt(count).getRejectFileName();
                    img.Source = setImgSource(fullpath, "sub");
                    num++;

                }

                else
                {
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                    num++;
                }

            }
            checkActive();
        }

        private void btnRejectFirst_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage = 1;
            counter.Content = currRejectPage;


            if (currRejectPage != 1 || countSelectedCategory(foldernameselected) > 10)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = true;
            }

            if (currRejectPage == 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = false;
            }


            changeRejectPage(currRejectPage, "overview", "");
        }

        private void btnRejectLast_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage = (countSelectedCategory(foldernameselected) - 1) / 10 + 1;
            counter.Content = currRejectPage;

            if (currRejectPage != 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = true;
            }

            if (countSelectedCategory(foldernameselected) < currRejectPage * 10 + 1)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = false;
            }

            changeRejectPage(currRejectPage, "overview", "");
        }

        //-----------------end pass page navigation-------------------

        private int countSelectedCategory(string selectedCategory)
        {
            int totalNum = 0;

            for (int count = 0; count < fImageFiles.Count; count++)
            {
                if (fImageFiles.ElementAt(count).getRejectFolderName() == selectedCategory)
                {
                    totalNum++;
                }
            }

            return totalNum;
        }

        private void btnShowSummary_Click(object sender, RoutedEventArgs e)
        {
            updateRejectFolderList();
            updateSummaryCount();
            SummaryBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void closeSummary_Click(object sender, RoutedEventArgs e)
        {
            SummaryBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnRevert_Click(object sender, RoutedEventArgs e)
        {
            revertimg = (Image)this.FindName("imgi");
            string tempfilename = System.IO.Path.GetFileName(revertimg.Source.ToString());
            string targetfile = "", ffolder = "";
            bool pfound = false, ffound = false;
            int pindex = -1, findex = -1;

            //check the source is from pass or fail
            for (int i = 0; i < pImageFiles.Count; i++)
            {
                if (pImageFiles.ElementAt(i) == tempfilename && activepreviewtab == "Pass")
                {
                    targetfile = tempfilename;
                    pfound = true;
                    pindex = i;
                    break;
                }
            }

            if (pfound == false)
            {
                for (int i = 0; i < fImageFiles.Count; i++)
                {
                    if (fImageFiles.ElementAt(i).getRejectFileName() == tempfilename && activepreviewtab == "Reject")
                    {
                        targetfile = tempfilename;
                        ffolder = fImageFiles.ElementAt(i).getRejectFolderName();
                        ffound = true;
                        findex = i;
                        break;
                    }
                }
            }
            string targetfolder = "";
            //remove from list and add back to new list
            //move the image


            if (pfound)
            {
                string pextpart = "";
                string pnamepart = "";
                bool dupdetect = false;
                if (File.Exists(uploadPath + "\\" + tempfilename) || imageFiles.Contains(pImageFiles.ElementAt(pindex)))
                {
                    MessageBoxResult mbresult;
                    mbresult = MessageBox.Show("Duplicate file name found at target location! Would you like to rename the image file name?\n" + uploadPath + "\\" + tempfilename, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (mbresult == MessageBoxResult.Yes)
                    {
                        pextpart = Path.GetExtension(uploadPath + "\\" + tempfilename);
                        pnamepart = Path.GetFileNameWithoutExtension(uploadPath + "\\" + tempfilename);
                        dupdetect = true;
                    }
                    else
                    {
                        return;
                    }
                }

                string newrename = "";

                if (dupdetect == true)
                {

                    bool renameprocess = true;
                    int incNum = 1;
                    do
                    {
                        if (File.Exists(uploadPath + "\\" + pnamepart + " (" + incNum + ")" + pextpart))
                        {
                            incNum++;
                        }
                        else
                        {
                            System.IO.File.Move(passPath + "\\" + tempfilename, uploadPath + "\\" + pnamepart + " (" + incNum + ")" + pextpart);
                            renameprocess = false;
                            newrename = pnamepart + " (" + incNum + ")" + pextpart;
                        }

                    } while (renameprocess);
                }
                else
                {
                    System.IO.File.Move(passPath + "\\" + tempfilename, uploadPath + "\\" + tempfilename);
                    newrename = tempfilename;
                }
                imageFiles.Insert(0, newrename);
                pImageFiles.RemoveAt(pindex);

                passcount--;
                passCount.Content = "Total Pass Count: " + passcount;

            }
            else if (ffound)
            {

                string fextpart = "";
                string fnamepart = "";
                bool fdupdetect = false;

                if (File.Exists(uploadPath + "\\" + tempfilename) || imageFiles.Contains(fImageFiles.ElementAt(findex).getRejectFileName()))
                {
                    MessageBoxResult mbresult;
                    mbresult = MessageBox.Show("Duplicate file name found at target location! Would you like to rename the image file name?\n" + uploadPath + "\\" + tempfilename, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (mbresult == MessageBoxResult.Yes)
                    {
                        fextpart = Path.GetExtension(uploadPath + "\\" + tempfilename);
                        fnamepart = Path.GetFileNameWithoutExtension(uploadPath + "\\" + tempfilename);
                        fdupdetect = true;

                    }
                    else
                    {
                        return;
                    }
                }


                string newrename = "";
                if (fdupdetect)
                {
                    bool renameprocess = true;
                    int incNum = 1;
                    do
                    {
                        if (File.Exists(uploadPath + "\\" + fnamepart + " (" + incNum + ")" + fextpart))
                        {
                            incNum++;
                        }
                        else
                        {
                            System.IO.File.Move(rejectPath + "\\" + ffolder + "\\" + tempfilename, uploadPath + "\\" + fnamepart + " (" + incNum + ")" + fextpart);
                            renameprocess = false;
                            newrename = fnamepart + " (" + incNum + ")" + fextpart;
                        }

                    } while (renameprocess);

                }
                else
                {
                    try
                    {
                        System.IO.File.Move(rejectPath + "\\" + ffolder + "\\" + tempfilename, uploadPath + "\\" + tempfilename);
                        newrename = tempfilename;
                    }
                    catch
                    {
                        MessageBox.Show("The target file is missing!");
                        imgi.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                        if (currTabName == "All")
                        {
                            changePage(currAllPage);
                        }
                        else if (currTabName == "Pass")
                        {
                            changePassPage(currPassPage);
                        }
                        else if (currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
                        {
                            changeRejectPage(currRejectPage, "special1", "");
                        }
                        return;
                    }

                }

                imageFiles.Insert(0, newrename);
                targetfolder = fImageFiles.ElementAt(findex).getRejectFolderName();
                fImageFiles.RemoveAt(findex);

                rejectcount--;
                rejectCount.Content = "Total Reject Count: " + rejectcount;
            }
            numProgress--;
            progressCount.Content = "Overall progress: " + numProgress + "/" + totalNum;

            //update counter
            updateSummaryCount();
            updateRejectFolderList();

            //update upload list
            //loadImage();

            //update view
            changePage(1);

            if (pfound && currTabName == "Pass")
            {
                changePassPage(1);
            }
            if (ffound && currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
            {
                if (currRejectFolder != "")
                {
                    changeRejectPage(1, "special", targetfolder);
                }
            }

            //after reset set to the first image of upload
            revertimg.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
            pfBtn = (Button)this.FindName("btnPass");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnReject");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnRevert");
            pfBtn.IsEnabled = false;
            setButtonStatus("btnDeleteImg", false);
            btnPass.Visibility = System.Windows.Visibility.Visible;
            btnReject.Visibility = System.Windows.Visibility.Visible;
            btnRevert.Visibility = System.Windows.Visibility.Collapsed;

            checkActive();
            saveFile();
        }

        private void updateSummaryCount()
        {
            string passpercent = "", rejectpercent = "";
            if (passcount != 0)
            {
                passpercent = ((passcount / (double)totalNum) * 100).ToString("F");
            }
            else
            {
                passpercent = "0.00";
            }
            if (rejectcount != 0)
            {
                rejectpercent = ((rejectcount / (double)totalNum) * 100).ToString("F");
            }
            else
            {
                rejectpercent = "0.00";
            }



            summary_total.Content = "Total Image: " + totalNum;
            summary_totalinspect.Content = "Total Inspected: " + numProgress;
            summary_pass.Content = "Total pass: " + passcount + " (" + passpercent + "%)";
            summary_fail.Content = "Total reject: " + rejectcount + " (" + rejectpercent + "%)";
        }

        public void saveFile()
        {
            proj.setProjDatetime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            proj.setPassCount(passcount);
            proj.setRejectCount(rejectcount);
            proj.setNumProgress(numProgress);
            proj.setTotalNum(totalNum);

            proj.setProjOriFile(imageFiles);
            proj.setProjPassFile(pImageFiles);
            proj.setProjRejectFile(fImageFiles);
            proj.setProjRejectFolder(rejectFolderList);

            Project.SerializeItem(proj);
        }

        public void loadFile(string savefile)
        {
            Project.DeserializeItem(proj, savefile);

            defPath = proj.getProjLocation();
            saveFilePath = defPath + "\\projData.dat";

            uploadPath = defPath + "\\uploaded";
            passPath = defPath + "\\pass";
            rejectPath = defPath + "\\reject";

            passcount = proj.getPassCount();
            rejectcount = proj.getRejectCount();
            numProgress = proj.getNumProgress();
            totalNum = proj.getTotalNum();


            if (proj.getProjOriFile() != null)
            {
                imageFiles = new List<string>(proj.getProjOriFile());
            }
            if (proj.getProjPassFile() != null)
            {
                pImageFiles = new List<string>(proj.getProjPassFile());
            }
            if (proj.getProjRejectFile() != null)
            {
                fImageFiles = new List<RejectFile>(proj.getProjRejectFile());
            }
            if (proj.getProjRejectFolder() != null)
            {
                rejectFolderList = new List<RejectFolder>(proj.getProjRejectFolder());
            }

        }

        private void returnhome_Click(object sender, RoutedEventArgs e)
        {
            saveFile();

            Menu opnmenu = new Menu();
            opnmenu.Show();

            this.Close();
        }

        private void setButtonStatus(string btnname, bool status)
        {

            pfBtn = (Button)this.FindName(btnname);
            pfBtn.IsEnabled = status;
        }

        private void updateCounter()
        {
            counter = (Label)this.FindName("progressCount");
            counter.Content = "Overall progress: " + numProgress + "/" + totalNum;

            counter = (Label)this.FindName("passCount");
            counter.Content = "Total Pass Count: " + passcount;

            counter = (Label)this.FindName("rejectCount");
            counter.Content = "Total Reject Count: " + rejectcount;
        }

        private void sRenameButton_Click(object sender, EventArgs e)
        {
            if (continueRenameProcess)
            {
                continueRenameProcess = false;
                sRenameButton.Content = "Resume";
            }
            else
            {
                continueRenameProcess = true;
                sRenameButton.Content = "Stop";
                sRenameButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new NextRenameDelegate(RenameFile));
            }
        }

        private void RenameFile()
        {
            if (currReCount == totalReCount)
            {
                ProgressBox_error.Visibility = System.Windows.Visibility.Collapsed;
                ProgressBox.Visibility = System.Windows.Visibility.Collapsed;

                uploadList.Clear();
                uploadFailList.Clear();

                currAllPage = 1;
                counter = (Label)this.FindName("allPage");
                counter.Content = currAllPage;
                changePage(currAllPage);

                setButtonStatus("btnAllPrev", false);
                setButtonStatus("btnAllFirst", false);


                if (imageFiles.Count > 10)
                {
                    setButtonStatus("btnAllNext", true);
                    setButtonStatus("btnAllLast", true);
                }
                else
                {
                    setButtonStatus("btnAllNext", false);
                    setButtonStatus("btnAllLast", false);
                }

                counter = (Label)this.FindName("progressCount");
                counter.Content = "Overall progress: " + numProgress + "/" + totalNum;


                saveFile();
                checkActive();
                continueRenameProcess = false;
                return;
            }

            if (File.Exists(uploadPath + "\\" + Path.GetFileName(uploadFailList.ElementAt(currReCount))))
            {
                bool ok = false;
                string renamefilename = "";
                int filenum = 1;

                continueRenameProcess = false;
                progresserrormsg.Text = "Duplicate image file name found!\nPlease rename(without the extention) or skip the file.";
                dupOriTextBlock.Text = uploadFailList.ElementAt(currReCount);
                string ext = Path.GetExtension(uploadPath + "\\" + Path.GetFileName(uploadFailList.ElementAt(currReCount)));
                do
                {
                    if (File.Exists(uploadPath + "\\" + Path.GetFileNameWithoutExtension(uploadFailList.ElementAt(currReCount)) + " (" + filenum + ")" + ext))
                    {
                        filenum++;
                    }
                    else
                    {
                        renamefilename = Path.GetFileNameWithoutExtension(uploadFailList.ElementAt(currReCount)) + " (" + filenum + ")";
                        ok = true;
                    }
                } while (ok == false);
                progress_InputTextBox.Text = renamefilename;
            }
            else
            {
                System.IO.File.Copy(uploadFailList.ElementAt(currReCount), uploadPath + "\\" + System.IO.Path.GetFileName(uploadFailList.ElementAt(currReCount)));
                imageFiles.Add(System.IO.Path.GetFileName(uploadFailList.ElementAt(currReCount)));
                currReCount++;
                totalNum++;
            }


            if (continueRenameProcess)
            {
                sRenameButton.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new NextRenameDelegate(this.RenameFile));
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {

            String input = progress_InputTextBox.Text;
            if (input.Length == 0)
            {
                MessageBox.Show("The field cannot be empty");
                return;
            }

            if (!File.Exists(uploadPath + "\\" + input + Path.GetExtension(uploadFailList.ElementAt(currReCount))))
            {
                try
                {
                    System.IO.File.Copy(uploadFailList.ElementAt(currReCount), uploadPath + "\\" + input + Path.GetExtension(uploadFailList.ElementAt(currReCount)));
                    imageFiles.Add(System.IO.Path.GetFileName(uploadFailList.ElementAt(currReCount)));
                    currReCount++;
                    totalNum++;
                    sRenameButton_Click(new object(), new EventArgs());
                }
                catch
                {
                    MessageBox.Show("The file name may not be used!");
                }

            }
            else
            {
                MessageBox.Show("The image file name \"" + input + Path.GetExtension(uploadFailList.ElementAt(currReCount)) + "\" already exists!");
            }

            // Clear InputBox.
            progress_InputTextBox.Text = String.Empty;

        }

        private void QuickRenameButton_Click(object sender, RoutedEventArgs e)
        {
            bool proceedNext = false, ok = false;
            int filenum = 1;
            string renamefilename = "", targetfilename = "";

            string ext = Path.GetExtension(uploadPath + "\\" + Path.GetFileName(uploadFailList.ElementAt(currReCount)));
            do
            {
                ok = false;
                filenum = 1;
                renamefilename = "";
                targetfilename = "";
                ext = Path.GetExtension(uploadPath + "\\" + Path.GetFileName(uploadFailList.ElementAt(currReCount)));
                do
                {
                    if (File.Exists(uploadPath + "\\" + Path.GetFileNameWithoutExtension(uploadFailList.ElementAt(currReCount)) + " (" + filenum + ")" + ext))
                    {
                        filenum++;
                    }
                    else
                    {
                        renamefilename = uploadPath + "\\" + Path.GetFileNameWithoutExtension(uploadFailList.ElementAt(currReCount)) + " (" + filenum + ")" + ext;
                        targetfilename = Path.GetFileNameWithoutExtension(uploadFailList.ElementAt(currReCount)) + " (" + filenum + ")" + ext;
                        ok = true;
                    }
                } while (ok == false);

                System.IO.File.Copy(uploadFailList.ElementAt(currReCount), renamefilename);
                imageFiles.Add(targetfilename);
                currReCount++;
                totalNum++;

                if (currReCount == totalReCount)
                {
                    ProgressBox_error.Visibility = System.Windows.Visibility.Collapsed;
                    ProgressBox.Visibility = System.Windows.Visibility.Collapsed;

                    uploadList.Clear();
                    uploadFailList.Clear();

                    currAllPage = 1;
                    counter = (Label)this.FindName("allPage");
                    counter.Content = currAllPage;
                    changePage(currAllPage);

                    setButtonStatus("btnAllPrev", false);
                    setButtonStatus("btnAllFirst", false);


                    if (imageFiles.Count > 10)
                    {
                        setButtonStatus("btnAllNext", true);
                        setButtonStatus("btnAllLast", true);
                    }
                    else
                    {
                        setButtonStatus("btnAllNext", false);
                        setButtonStatus("btnAllLast", false);
                    }

                    counter = (Label)this.FindName("progressCount");
                    counter.Content = "Overall progress: " + numProgress + "/" + totalNum;


                    saveFile();
                    checkActive();
                    continueRenameProcess = false;
                    proceedNext = true;
                }
            } while (proceedNext == false);
            MessageBox.Show("Rename Completed!");
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            currReCount++;
            sRenameButton_Click(new object(), new EventArgs());
            progress_InputTextBox.Text = String.Empty;
        }

        private void SkipAllButton_Click(object sender, RoutedEventArgs e)
        {
            continueRenameProcess = false;
            progress_InputTextBox.Text = String.Empty;

            ProgressBox_error.Visibility = System.Windows.Visibility.Collapsed;
            ProgressBox.Visibility = System.Windows.Visibility.Collapsed;

            uploadList.Clear();
            uploadFailList.Clear();

            currAllPage = 1;
            counter = (Label)this.FindName("allPage");
            counter.Content = currAllPage;
            changePage(currAllPage);

            setButtonStatus("btnAllPrev", false);
            setButtonStatus("btnAllFirst", false);


            if (imageFiles.Count > 10)
            {
                setButtonStatus("btnAllNext", true);
                setButtonStatus("btnAllLast", true);
            }
            else
            {
                setButtonStatus("btnAllNext", false);
                setButtonStatus("btnAllLast", false);
            }

            counter = (Label)this.FindName("progressCount");
            counter.Content = "Overall progress: " + numProgress + "/" + totalNum;


            saveFile();
            checkActive();
        }

        private void sUploadButton_Click(object sender, EventArgs e)
        {
            if (continueProcess)
            {
                continueProcess = false;
                sUploadButton.Content = "Resume";
            }
            else
            {
                continueProcess = true;
                sUploadButton.Content = "Stop";
                sUploadButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new NextUploadDelegate(UploadFile));
            }
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(activemissingfile);
        }

        private void UploadFile()
        {
            if (currUpCount == totalUpCount)
            {
                continueProcess = false;
                if (uploaderror)
                {
                    currReCount = 0;
                    totalReCount = uploadFailList.Count();


                    MessageBoxResult mbresult;
                    mbresult = MessageBox.Show("Duplicate file name found! Would you like to rename the image files?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (mbresult == MessageBoxResult.Yes)
                    {
                        ProgressBox_progress.Visibility = System.Windows.Visibility.Collapsed;
                        ProgressBox_error.Visibility = System.Windows.Visibility.Visible;
                        sRenameButton_Click(new object(), new EventArgs());
                    }
                    else
                    {
                        afterUpload();
                    }


                }
                else
                {
                    afterUpload();

                }
                return;
            }

            bool tempstatus = false;

            for (int i = 0; i < imageFiles.Count; i++)
            {
                if (imageFiles.ElementAt(i) == Path.GetFileName(uploadList.ElementAt(currUpCount)))
                {
                    uploadFailList.Add(uploadList.ElementAt(currUpCount));
                    currUpCount++;
                    progressnum.Text = currUpCount + "/" + totalUpCount;
                    uploaderror = true;
                    tempstatus = true;
                    break;
                }
            }
            if (!tempstatus)
            {
                if (File.Exists(uploadPath + "\\" + Path.GetFileName(uploadList.ElementAt(currUpCount))))
                {
                    uploadFailList.Add(uploadList.ElementAt(currUpCount));
                    currUpCount++;
                    progressnum.Text = currUpCount + "/" + totalUpCount;
                    uploaderror = true;
                }
                else
                {
                    System.IO.File.Copy(uploadList.ElementAt(currUpCount), uploadPath + "\\" + System.IO.Path.GetFileName(uploadList.ElementAt(currUpCount)));
                    imageFiles.Add(System.IO.Path.GetFileName(uploadList.ElementAt(currUpCount)));
                    currUpCount++;
                    progressnum.Text = currUpCount + "/" + totalUpCount;
                    totalNum++;

                }
            }



            if (continueProcess)
            {
                Thread.Sleep(100);
                sUploadButton.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new NextUploadDelegate(this.UploadFile));
            }
        }

        public void afterUpload()
        {
            progressmsg.Text = "Image(s) has been successfully uploaded!";
            MessageBox.Show("Image(s) has been successfully uploaded!");
            ProgressBox_progress.Visibility = System.Windows.Visibility.Collapsed;
            ProgressBox.Visibility = System.Windows.Visibility.Collapsed;

            uploadList.Clear();
            uploadFailList.Clear();

            currAllPage = 1;
            counter = (Label)this.FindName("allPage");
            counter.Content = currAllPage;
            changePage(currAllPage);

            setButtonStatus("btnAllPrev", false);
            setButtonStatus("btnAllFirst", false);


            if (imageFiles.Count > 10)
            {
                setButtonStatus("btnAllNext", true);
                setButtonStatus("btnAllLast", true);
            }
            else
            {
                setButtonStatus("btnAllNext", false);
                setButtonStatus("btnAllLast", false);
            }

            counter = (Label)this.FindName("progressCount");
            counter.Content = "Overall progress: " + numProgress + "/" + totalNum;


            saveFile();
            checkActive();
        }

        public string findRejectFileName(int pagenum, int selectedposition, string rejectfoldername)
        {
            string foundfilename = "";

            List<RejectFile> tempreject = new List<RejectFile>();

            for (int i = 0; i < fImageFiles.Count; i++)
            {
                if (fImageFiles.ElementAt(i).getRejectFolderName() == rejectfoldername)
                {
                    tempreject.Add(new RejectFile(fImageFiles.ElementAt(i).getRejectFolderName(), fImageFiles.ElementAt(i).getRejectFileName()));

                }
            }

            foundfilename = tempreject.ElementAt(tempreject.Count - selectedposition + (pagenum * 10 - 10)).getRejectFileName();


            return foundfilename;
        }

        public void handleImageMissing(string selectedImage)
        {
            //disable and hide revert, pass and reject
            pfBtn = (Button)this.FindName("btnPass");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnReject");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnRevert");
            pfBtn.IsEnabled = false;
            btnPass.Visibility = System.Windows.Visibility.Collapsed;
            btnReject.Visibility = System.Windows.Visibility.Collapsed;
            btnRevert.Visibility = System.Windows.Visibility.Collapsed;
            //show button
            pfBtn = (Button)this.FindName("btnInfo");
            pfBtn.IsEnabled = true;
            btnInfo.Visibility = System.Windows.Visibility.Visible;



            if (!selectedImage.Contains("P") && !selectedImage.Contains("F"))
            {
                position = Int32.Parse(selectedImage.Substring(3));
            }
            else if (selectedImage.Contains("P"))
            {
                position = Int32.Parse(selectedImage.Substring(4));
            }
            else if (selectedImage.Contains("F"))
            {
                position = Int32.Parse(selectedImage.Substring(4));
            }
            tabname = currTabName;
            if (currTabName == "All")
            {
                pagenum = currAllPage;
                activemissingfile = "uploaded\\" + imageFiles.ElementAt(pagenum * 10 - 10 + position - 1);
            }
            else if (currTabName == "Pass")
            {
                pagenum = currPassPage;
                activemissingfile = "pass\\" + pImageFiles.ElementAt(pImageFiles.Count - position + (pagenum * 10 - 10));
            }
            else if (currTabName == "Reject")
            {

                pagenum = currRejectPage;

                if (currRejectFolder != null || currRejectFolder.Length == 0)
                {
                    rejectName = currRejectFolder;
                }
                activemissingfile = "reject\\" + currRejectFolder + "\\" + findRejectFileName(pagenum, position, rejectName);
            }
            selImg = (Image)this.FindName("imgi");
            selImg.Source = setImgSource("pack://application:,,,/Resources/imagemissing.png", "main");
            selImg = (Image)this.FindName(selectedImage);
            selImg.Source = setImgSource("pack://application:,,,/Resources/imagemissing.png", "sub");
            checkActive();
        }

        private void btnDeleteImg_Click(object sender, RoutedEventArgs e)
        {
            string[] arraystring;
            int index = 0;

            MessageBoxResult mbresult;
            mbresult = MessageBox.Show("Would you like to delete the image?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (mbresult == MessageBoxResult.No)
            {
                return;
            }

            if (System.IO.Path.GetFileName(imgi.Source.ToString()) == "imagemissing.png")
            {
                arraystring = activemissingfile.Split('\\');
                switch (arraystring[0])
                {
                    case "uploaded":
                        index = imageFiles.IndexOf(arraystring[1]);
                        imageFiles.RemoveAt(index);
                        totalNum--;

                        break;
                    case "pass":
                        index = pImageFiles.IndexOf(arraystring[1]);
                        pImageFiles.RemoveAt(index);
                        totalNum--;
                        numProgress--;
                        passcount--;

                        break;
                    case "reject":
                        for (int i = 0; i < fImageFiles.Count; i++)
                        {
                            if (fImageFiles.ElementAt(i).getRejectFolderName() == arraystring[1] && fImageFiles.ElementAt(i).getRejectFileName() == arraystring[2])
                            {
                                index = i;
                                break;
                            }
                        }
                        fImageFiles.RemoveAt(index);
                        totalNum--;
                        numProgress--;
                        rejectcount--;

                        break;
                }


                updateCounter();
                updateRejectFolderList();


                currAllPage = 1;
                counter = (Label)this.FindName("allPage");
                counter.Content = currAllPage;
                changePage(1);

                if (imageFiles.Count < 11)
                {
                    setButtonStatus("btnAllNext", false);
                    setButtonStatus("btnAllLast", false);
                }
                else
                {
                    setButtonStatus("btnAllNext", true);
                    setButtonStatus("btnAllLast", true);
                }
                setButtonStatus("btnAllPrev", false);
                setButtonStatus("btnAllFirst", false);

                if (currTabName == "Pass")
                {
                    currPassPage = 1;
                    counter = (Label)this.FindName("passPage");
                    counter.Content = currPassPage;
                    changePassPage(currPassPage);

                    if (pImageFiles.Count < 11)
                    {
                        setButtonStatus("btnPassNext", false);
                        setButtonStatus("btnPassLast", false);
                    }
                    else
                    {
                        setButtonStatus("btnPassNext", true);
                        setButtonStatus("btnPassLast", true);
                    }
                    setButtonStatus("btnPassPrev", false);
                    setButtonStatus("btnPassFirst", false);
                }
                else if (currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
                {
                    if (currRejectFolder != "" && arraystring[0] == "reject")
                    {
                        currRejectPage = 1;
                        counter = (Label)this.FindName("rejectPage");
                        counter.Content = currRejectPage;
                        changeRejectPage(currRejectPage, "special1", "");


                        setButtonStatus("btnRejectPrev", false);
                        setButtonStatus("btnRejectFirst", false);


                        if (countSelectedCategory(arraystring[1]) > 10)
                        {
                            setButtonStatus("btnRejectNext", true);
                            setButtonStatus("btnRejectLast", true);
                        }
                        else
                        {
                            setButtonStatus("btnRejectNext", false);
                            setButtonStatus("btnRejectLast", false);
                        }

                    }
                }
                imgi.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                setButtonStatus("btnPass", false);
                setButtonStatus("btnReject", false);
                setButtonStatus("btnDeleteImg", false);
                setButtonStatus("btnRevert", false);
                btnRevert.Visibility = Visibility.Collapsed;
                btnPass.Visibility = Visibility.Collapsed;
                btnReject.Visibility = Visibility.Collapsed;
                btnInfo.Visibility = Visibility.Collapsed;


                //handle when in all page, set first image, check if still gt img
                //handle when in pass/reject page, set imgi to noimg
                saveFile();
                checkActive();
            }
            else if (System.IO.Path.GetFileName(imgi.Source.ToString()) != "noimg.png")
            {
                string directory = "";
                int indexx = 0;
                directory = Path.GetFileName(Path.GetDirectoryName(imgi.Source.ToString()));
                if (directory == "uploaded")
                {
                    File.Delete(uploadPath + "\\" + Path.GetFileName(imgi.Source.ToString()));
                    indexx = imageFiles.IndexOf(Path.GetFileName(imgi.Source.ToString()));
                    imageFiles.RemoveAt(indexx);
                    totalNum--;
                }
                else if (directory == "pass")
                {
                    File.Delete(passPath + "\\" + Path.GetFileName(imgi.Source.ToString()));
                    indexx = pImageFiles.IndexOf(Path.GetFileName(imgi.Source.ToString()));
                    pImageFiles.RemoveAt(indexx);
                    totalNum--;
                    numProgress--;
                    passcount--;
                }
                else
                {
                    File.Delete(rejectPath + "\\" + directory + "\\" + Path.GetFileName(imgi.Source.ToString()));
                    for (int i = 0; i < fImageFiles.Count; i++)
                    {
                        if (fImageFiles.ElementAt(i).getRejectFolderName() == directory && fImageFiles.ElementAt(i).getRejectFileName() == Path.GetFileName(imgi.Source.ToString()))
                        {
                            index = i;
                            break;
                        }
                    }
                    fImageFiles.RemoveAt(index);
                    totalNum--;
                    numProgress--;
                    rejectcount--;

                }

                updateCounter();
                updateRejectFolderList();


                currAllPage = 1;
                counter = (Label)this.FindName("allPage");
                counter.Content = currAllPage;
                changePage(1);

                if (imageFiles.Count < 11)
                {
                    setButtonStatus("btnAllNext", false);
                    setButtonStatus("btnAllLast", false);
                }
                else
                {
                    setButtonStatus("btnAllNext", true);
                    setButtonStatus("btnAllLast", true);
                }
                setButtonStatus("btnAllPrev", false);
                setButtonStatus("btnAllFirst", false);

                if (currTabName == "Pass")
                {
                    currPassPage = 1;
                    counter = (Label)this.FindName("passPage");
                    counter.Content = currPassPage;
                    changePassPage(currPassPage);

                    if (pImageFiles.Count < 11)
                    {
                        setButtonStatus("btnPassNext", false);
                        setButtonStatus("btnPassLast", false);
                    }
                    else
                    {
                        setButtonStatus("btnPassNext", true);
                        setButtonStatus("btnPassLast", true);
                    }
                    setButtonStatus("btnPassPrev", false);
                    setButtonStatus("btnPassFirst", false);
                }
                else if (currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
                {
                    if (currRejectFolder != "" && directory != "uploaded" && directory != "pass")
                    {
                        currRejectPage = 1;
                        counter = (Label)this.FindName("rejectPage");
                        counter.Content = currRejectPage;
                        changeRejectPage(currRejectPage, "special1", "");

                        setButtonStatus("btnRejectPrev", false);
                        setButtonStatus("btnRejectFirst", false);


                        if (countSelectedCategory(directory) > 10)
                        {
                            setButtonStatus("btnRejectNext", true);
                            setButtonStatus("btnRejectLast", true);
                        }
                        else
                        {
                            setButtonStatus("btnRejectNext", false);
                            setButtonStatus("btnRejectLast", false);
                        }

                    }
                }
                imgi.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                setButtonStatus("btnPass", false);
                setButtonStatus("btnReject", false);
                setButtonStatus("btnDeleteImg", false);
                setButtonStatus("btnRevert", false);

                //handle when in all page, set first image, check if still gt img
                //handle when in pass/reject page, set imgi to noimg
                saveFile();
                checkActive();


            }
        }

        private void btnManageReject_Click(object sender, RoutedEventArgs e)
        {

            ManageRejectBox.Visibility = Visibility.Visible;
            ManageRejectMenu.Visibility = Visibility.Visible;
            updateRejectFolderList();
        }

        private void cancelManageReject_Click(object sender, RoutedEventArgs e)
        {
            ManageRejectMenu.Visibility = Visibility.Collapsed;
            ManageRejectBox.Visibility = Visibility.Collapsed;
        }

        private void renameManageReject_Click(object sender, RoutedEventArgs e)
        {
            if (rejectManageBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a folder to proceed");
                return;

            }

            RejectFolder temp = (RejectFolder)rejectManageBox.SelectedItem;
            selectedManageReject = temp.getRejectFolderName();
            //selectedManageReject = rejectManageBox.SelectedItem.ToString();
            ManageRejectMenu.Visibility = Visibility.Collapsed;
            orifoldername.Text = selectedManageReject;
            RenameRejectFolder.Visibility = Visibility.Visible;



        }

        private void deleteManageReject_Click(object sender, RoutedEventArgs e)
        {
            if (rejectManageBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a folder to proceed");
                return;

            }

            RejectFolder temp = (RejectFolder)rejectManageBox.SelectedItem;
            selectedManageReject = temp.getRejectFolderName();
            //selectedManageReject = rejectManageBox.SelectedItem.ToString();
            ManageRejectMenu.Visibility = Visibility.Collapsed;
            managedefdelname.Text = selectedManageReject;
            DeleteRejectFolder.Visibility = Visibility.Visible;

            //revert all or delete all
        }

        private void btnRenameDefRename_Click(object sender, RoutedEventArgs e)
        {

            String input = newfoldername.Text;
            if (input.Length == 0)
            {
                MessageBox.Show("Please enter a name");
                return;
            }
            try
            {
                if (!Directory.Exists(rejectPath + "\\" + input))
                {
                    Directory.CreateDirectory(rejectPath + "\\" + input);
                    for (int i = 0; i < fImageFiles.Count; i++)
                    {
                        if (fImageFiles.ElementAt(i).getRejectFolderName() == selectedManageReject)
                        {

                            File.Move(rejectPath + "\\" + selectedManageReject + "\\" + fImageFiles.ElementAt(i).getRejectFileName(), rejectPath + "\\" + input + "\\" + fImageFiles.ElementAt(i).getRejectFileName());
                            fImageFiles.ElementAt(i).setRejectFolderName(input);
                        }
                    }
                    saveFile();
                    Directory.Delete(rejectPath + "\\" + selectedManageReject);
                    RenameRejectFolder.Visibility = System.Windows.Visibility.Collapsed;
                    ManageRejectBox.Visibility = Visibility.Collapsed;
                    newfoldername.Text = String.Empty;
                    updateRejectFolderList();

                }
                else
                {
                    MessageBox.Show("The folder name already exists or cannot be used!");
                }
            }
            catch
            {
                MessageBox.Show("The folder name cannot be used");
            }
        }

        private void btnRenameDefCancel_Click(object sender, RoutedEventArgs e)
        {
            updateRejectFolderList();
            RenameRejectFolder.Visibility = Visibility.Collapsed;
            ManageRejectMenu.Visibility = Visibility.Visible;
        }

        private void btnDeleteDefDel_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(rejectPath + "\\" + selectedManageReject, true);
            int tempnum = countSelectedCategory(selectedManageReject);

            totalNum -= tempnum;
            numProgress -= tempnum;
            rejectcount -= tempnum;
            updateCounter();
            updateRejectFolderList();
            saveFile();
            DeleteRejectFolder.Visibility = Visibility.Collapsed;
            ManageRejectBox.Visibility = Visibility.Collapsed;
        }
        private void btnDeleteDefCancel_Click(object sender, RoutedEventArgs e)
        {
            DeleteRejectFolder.Visibility = Visibility.Collapsed;
            ManageRejectMenu.Visibility = Visibility.Visible;

        }


        public void dsw_closed(object sender, EventArgs e) //if user closes the window
        {

            //update modified time in txtfile
            //get text file path
            string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + "ProjectDetails.txt";

            //updated content
            string content = fn + ","
                + fd + ","
                + fp + ","
                + fc + ","
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //update modified time in text file
            //change old line with new updated line
            lineChanger(content, path, projindex);

        }

        //ADD THIS
        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            //function to replace old line with new line
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        public bool checkReject(string foldername, string filename)
        {
            bool dup = false;
            for (int i = 0; i < fImageFiles.Count; i++)
            {
                if (fImageFiles.ElementAt(i).getRejectFolderName() == foldername && fImageFiles.ElementAt(i).getRejectFileName() == filename)
                {
                    dup = true;
                }
            }
            return dup;
        }
    }
}