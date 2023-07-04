namespace Common.Util.Math.Path
{
	public interface IGraph<TC>
	{
		/**
		 * retrieve distance between specified cells, for example distance between cells on 2d grid
		 * could be calculated using formula:
		 * (Math.abs(start.x - end.x) + Math.abs(start.y - end.y));
		 */
		int Distance(TC from, TC to);

		/**
		 * retrieve number of sibling cells that are traversable from specified cell
		 */
		int GetSiblingCount(TC c);

		/**
		 * retrieve sibling cell at specified index for given cell
		 */
		TC GetSibling(TC c, int index);
	}
}
