using System;
using System.Text;
using System.Collections.Generic;

namespace SharpMod.Menu
{
	public class Menu : SimpleMenu
	{
		public const int maximumItemsPerPage = 8;
		private int itemsPerPage = maximumItemsPerPage;
		public int ItemsPerPage {
			get { return itemsPerPage; }
			set { itemsPerPage = (value < 1 ? 1 : (value > maximumItemsPerPage ? maximumItemsPerPage : value )); }
		}

		public Menu(string text)
			: this(text, true)
		{

		}

		public Menu(string text, bool enabled)
			: base(text, enabled)
		{
		}


		private int GetItemIndex(int page)
		{
			return itemsPerPage * page;
		}

		private int GetPageCount(int items)
		{
			return (items / itemsPerPage);
		}

		public override MenuInfo GetMenuInfo(Player player)
		{
			StringBuilder sb = new StringBuilder();
			List<Item> acc = new List<Item>();

			int i = 0, j = 0;;
			short keys = 0;
			int page = 0;

			if (player.menu == this) {
				page = player.menu_page;
			} else {
				player.menu      = this;
				player.menu_page = 0;
			}

			int max = 0;
			MenuIterator(player, acc, GetItemIndex(page), ref i, GetItemIndex(page) + itemsPerPage, ref max);
			int maxPage = GetPageCount(max);

			player.menu_items = acc.ToArray();

			sb.Append(Text);
			sb.Append(String.Format(" ({0}/{1})\n\n", page + 1, maxPage + 1));

			foreach (Item item in acc) {
				AddItemText(sb, j, item);
				if (item.Enabled) {
					keys |= (short)(1 << j);
				}
				if (item.Selectable) {
					j++;
				}
			}
			sb.Append("\n\n");

			if (page > 0) {
				keys |= (short)(1 << 8); // back
				sb.Append("9. Back\n");
			}

			if (page < maxPage) {
				// if there are some more pages, hit the next
				// page counter
				keys |= (short)(1 << 9); // next
				sb.Append("0. Next\n");
			}

			return new MenuInfo(keys, DisplayTime, sb.ToString());
		}

		public override bool DoSelect(Player player, int index)
		{
			switch (index) {
			case 8:
				if (player.menu_page > 0) {
					player.menu_page--;
				}
				this.Show(player);
				return true;
			case 9:
				player.menu_page++;
				this.Show(player);
				return true;
			default:
				return base.DoSelect(player, index);
			}
		}
	}
}

