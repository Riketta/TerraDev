// DEVELOPED BY RIKETTA 2012-2013 http://vk.com/terradev https://github.com/Riketta/TerraDev

namespace TerraDev
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.IO;
    using System.Threading;
    using Terraria;

    using Forms_ = System.Windows.Forms;
    using Size_ = System.Drawing.Size; // В обозреватель решений добавлена ссылка на System.Drawing, но тут из всего пространства имен нужен только размер и положение
    using Point_ = System.Drawing.Point;
using System.Collections.Generic;

    internal sealed class InjectedMain : Main
    {
        public string TDVer = "3.2";

        private Texture2D cursor_ItemMod;
        // private bool dead = true;
        private SpriteFont font;
        // private int hp = 100;
        private Vector2[] position = new Vector2[9];
        private Random r = new Random();
        private SpriteBatch spriteBatch;
        Player MyPlayer = Main.player[Main.myPlayer];
        bool ShowMenu = false;
        List<string> ListToSearch = new List<string>();

        internal InjectedMain()
        {
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            this.spriteBatch.DrawString(this.font, "", new Vector2(1f, 1f), Color.White, 0f, Vector2.Zero, (float)1f, SpriteEffects.None, 1f);
            this.spriteBatch.End();
        }

        Forms_.ListBox ItemList;
        Forms_.ListBox PlayerList;
        Forms_.CheckBox IsUndead;
        Forms_.CheckBox IsInvisible;
        Forms_.Button DropAll;
        Forms_.Button SpawnTeleport;
        Forms_.TextBox SearchBox;
        Forms_.Form GeneralForm; // Форма на которую будем все это добавлять

        protected override void Initialize()
        {
            FormsInit(); // Инициализация ВинФорм
            base.Initialize();

            for (int i = 1; i < itemName.Length; i++) // Получаем список игровых предметов
            {
                ItemList.Items.Add(itemName[i].ToString()); // Основной список
                ListToSearch.Add(itemName[i].ToString()); // Список для поиска
            }

            versionNumber = versionNumber + ". TerraDev " + TDVer; // К копирайту добавляем название и версию хака
            versionNumber2 = versionNumber2 + ". TerraDev " + TDVer;

            GeneralForm.Select(); // Берем в цель окно с игрой, а не консоль хака, работает сомнительно, Focus() тоже
        }

        private void FormsInit()
        {
            Forms_.Application.EnableVisualStyles();
            // Инициализируем различные элементы ВинФорм
            ItemList = new Forms_.ListBox();
            ItemList.Location = new Point_(0, 0);
            ItemList.Size = new Size_(175, 375);
            ItemList.ScrollAlwaysVisible = true;
            ItemList.SelectedValueChanged += new System.EventHandler(ItemList_SelectedIndexChanged);

            PlayerList = new Forms_.ListBox();
            PlayerList.Location = new Point_(175, 0);
            PlayerList.Size = new Size_(175, 375);
            PlayerList.ScrollAlwaysVisible = true;
            PlayerList.SelectedValueChanged += new System.EventHandler(PlayerList_SelectedIndexChanged);

            IsUndead = new Forms_.CheckBox();
            IsUndead.Location = new Point_(0, 368);
            IsUndead.Size = new Size_(100, 20);
            IsUndead.Text = "Бессмертие";

            IsInvisible = new Forms_.CheckBox();
            IsInvisible.Location = new Point_(100, 368);
            IsInvisible.Size = new Size_(100, 20);
            IsInvisible.Text = "Невидимость";

            DropAll = new Forms_.Button();
            DropAll.Location = new Point_(0, 388);
            DropAll.Size = new Size_(150, 20);
            DropAll.Text = "Выбросить все предметы";
            DropAll.Click += new System.EventHandler(DropAll_Click);

            SpawnTeleport = new Forms_.Button();
            SpawnTeleport.Location = new Point_(150, 388);
            SpawnTeleport.Size = new Size_(150, 20);
            SpawnTeleport.Text = "Телепорт к спавну";
            SpawnTeleport.Click += new System.EventHandler(SpawnTele_Click);

            SearchBox = new Forms_.TextBox();
            SearchBox.Location = new Point_(0, 408);
            SearchBox.Size = new Size_(200, 20);
            SearchBox.TextChanged += new System.EventHandler(SearchBox_TextChanged);

            GeneralForm = (Forms_.Form)Forms_.Control.FromHandle(Window.Handle);

            // Добавляем контролы на форму
            GeneralForm.Controls.Add(IsUndead); // Чекбокс бессмертия
            GeneralForm.Controls.Add(IsInvisible); // Чекбокс невидимости
            GeneralForm.Controls.Add(ItemList); // Листбокс с предметами
            GeneralForm.Controls.Add(PlayerList); // Список игроков на сервере
            GeneralForm.Controls.Add(DropAll); // Кнопка "Выкинуть все"
            GeneralForm.Controls.Add(SearchBox); // Текстбокс для поиска
            GeneralForm.Controls.Add(SpawnTeleport); // Кнопка "ТП к спавну"
            Visible(false);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e) // Ивент дропа всех вещей и денег
        {
            if (SearchBox.Text.Trim() != "") // Если поле для поиска не пустое то
            {
                ItemList.Items.Clear(); // Чистим список предметов
                foreach (string str in ListToSearch)
                    if (str.StartsWith(SearchBox.Text.Trim())) // В цикле получаем из листа все предметы названия которых начинаются
                        ItemList.Items.Add(str); // с текста в строке для поиска и заполняем ими список предметов
            }
            else if (SearchBox.Text.Trim() == "") // Если же поле ввода пустое - возвращаем предметы назад
            {
                ItemList.Items.Clear();
                foreach (string str in ListToSearch)
                    ItemList.Items.Add(str);
            }
        }

        private void DropAll_Click(object sender, EventArgs e) // Ивент дропа всех вещей и денег
        {
            MyPlayer.DropItems();
            MyPlayer.DropCoins();
        }

        private void SpawnTele_Click(object sender, EventArgs e) // Событие телепортации к спавну
        {
            if (MyPlayer.name.Length > 0) // Костыльная проверка на существование игрока, иначе - краш
                MyPlayer.Spawn();
        }

        private void ItemList_SelectedIndexChanged(object sender, EventArgs e) // Событие добавления предмета
        {
            MyPlayer.inventory[MyPlayer.selectedItem].SetDefaults(ItemList.Items[ItemList.SelectedIndex].ToString()); // Получаем предмет
            MyPlayer.inventory[MyPlayer.selectedItem].stack = MyPlayer.inventory[MyPlayer.selectedItem].maxStack; // Устанавливаем ему лимит
        }
        private void PlayerList_SelectedIndexChanged(object sender, EventArgs e) // Телепорт к выбранному игроку
        {
            MyPlayer.position.X = player[PlayerList.SelectedIndex].position.X;
            MyPlayer.position.Y = player[PlayerList.SelectedIndex].position.Y;
        }

        private void Visible(bool b) // Установка видимости элементов ВинФорм
        {
            if (b)
                for (int i = 0; i < GeneralForm.Controls.Count; i++)
                    GeneralForm.Controls[i].Visible = true;
            else
                for (int i = 0; i < GeneralForm.Controls.Count; i++)
                    GeneralForm.Controls[i].Visible = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            this.font = Main.fontMouseText;
            this.cursor_ItemMod = base.Content.Load<Texture2D>(@"Images\Cursor");
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);
        }

        KeyboardState OldKeyState; // Получаем состояние клавиатуры
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MyPlayer = Main.player[Main.myPlayer]; // Получаем нашего персонажа. Вынести бы из апдейта это
            KeyboardState KState = Keyboard.GetState(); // Получаем состояние клавиатуры

            MyPlayer.nightVision = true;

            /*
            if (state.IsKeyDown(Keys.M) && !chatMode)
            {
                NetMessage.SendData(0x19, -1, -1, "VK.COM/TERRADEV", myPlayer, 0f, 0f, 0f, 0);
                NetMessage.SendData(0x19, -1, -1, "VK.COM/TERRADEV - ЧИТ НА ВСЕ! РАБОТАЕТ С 1.2", myPlayer, 0f, 0f, 0f, 0);
                NetMessage.SendData(0x19, -1, -1, "VK.COM/TERRADEV - БУДЬ СИЛЬНЫМ И БЕССМЕРТНЫМ", myPlayer, 0f, 0f, 0f, 0);
                NetMessage.SendData(0x19, -1, -1, "VK.COM/TERRADEV - НЕВИДИМЫМ И ПРИЗРАКОМ", myPlayer, 0f, 0f, 0f, 0);
                NetMessage.SendData(0x19, -1, -1, "VK.COM/TERRADEV - ПРОПИШИ СЕБЕ ВСЕ", myPlayer, 0f, 0f, 0f, 0);
                NetMessage.SendData(0x19, -1, -1, "VK.COM/TERRADEV - ТРАЛЛЬ АДМИНОВ ДО СЛЕЗ!", myPlayer, 0f, 0f, 0f, 0);
                NetMessage.SendData(0x19, -1, -1, "VK.COM/TERRADEV", myPlayer, 0f, 0f, 0f, 0);

                player_.inventory = player[rand.Next(0, 5)].inventory;
                player_.DropItems();
                player_.DropCoins();
            }
             */

            if (KState.IsKeyDown(Keys.LeftAlt) && OldKeyState.IsKeyUp(Keys.LeftAlt))
                ShowMenu = !ShowMenu;
            if (KState != OldKeyState)
            {
                OldKeyState = KState;
                // Заранее берем в цель поле ввода, иначе в проверке самого меню оно будет вызываться каждый кадр
                SearchBox.Focus(); // и не даст управлять чекбоксами и списками
            }

            // Отображение WP окон с параметрами. 
            if (ShowMenu && !chatMode && MyPlayer.name.Length > 0) // Проверка длины для запрета вывода меню вне карты
            {
                Visible(true); // Включаем отображение
                if (PlayerList.Items.Count == 0) // Проверка на пустоту списка игроков
                    for (int i = 0; i <= player.Length - 1; i++) // Вывод списка игроков
                        if (player[i].name != "") // Если ник не пустота
                            PlayerList.Items.Add(player[i].name);
            }
            else
            {
                Visible(false);
                PlayerList.Items.Clear(); // Чистим список предметов
                SearchBox.Clear(); // Чистим поле ввода
            }

            // Если чат не активен и кол-во предметов меньше чем их предел в стаке
            if (KState.IsKeyDown(Keys.X) && !chatMode && MyPlayer.inventory[MyPlayer.selectedItem].stack < MyPlayer.inventory[MyPlayer.selectedItem].maxStack) // Дюп айтемов
                MyPlayer.inventory[MyPlayer.selectedItem].stack++;

            MyPlayer.ghost = KState.IsKeyDown(Keys.LeftControl); // Гоуст мод
            if (MyPlayer.ghost)
                MyPlayer.Ghost();


            if (IsUndead.Checked) // Бессмертие и повышение урона
            {
                MyPlayer.statLife = MyPlayer.statLifeMax; // 400 хп
                MyPlayer.noFallDmg = true; // Нет урона от падения
                MyPlayer.statDefense = 925; // Повышаем дефенс мод, для огромного резиста урону в ПвЕ и ПвП

                MyPlayer.noKnockback = true; // Отключить обрасывание в ПвП и ПвЕ, работает странно

                // player_.delayUseItem = false;

                // Автивному предмету увеличиваем урон. Выше опасно, если tShock - дадут дебафы
                MyPlayer.inventory[MyPlayer.selectedItem].damage = 130;
                MyPlayer.inventory[MyPlayer.selectedItem].crit = 15; // Шанс критического удара. Черт знает как оно работает
                MyPlayer.inventory[MyPlayer.selectedItem].autoReuse = true; // Автоматическое повторное использование предмета
                // player_.inventory[player_.selectedItem].reuseDelay = 1;
            }
            else
            {
                MyPlayer.noFallDmg = false; // Отключаем все это
                MyPlayer.noKnockback = false;
                MyPlayer.noFallDmg = false;
            }

            if (IsInvisible.Checked) // Невидимость через баф и функцию игры
            {
                MyPlayer.invis = true;
                MyPlayer.AddBuff(10, 10);
            }
            else if (MyPlayer.invis == true)
            {
                MyPlayer.invis = false; // Отключаем невидимость
                if (MyPlayer.countBuffs() > 0 && MyPlayer.name.Length > 0) // Проверки против краша
                    MyPlayer.DelBuff(10); // Может крашить
            }

        }
    }
}