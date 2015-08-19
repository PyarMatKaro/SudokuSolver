To Do

Generic parameters
 Tiles should be added to candidates
 Solution should be an array of candidates
 Tiles in row have candidates
 CheckSelectCandidate etc

Design problem - the grid HAS a solver which doubles as pencil mark manager, the controller should USE a solver

Use improvements to solver in other projects

Instrument solver performance, and variants such as casting out impossibles

Try using contradiction hints on removing a row

Killer, others

Show contradiction hints with black pencil marks even when they are disabled, as well as the red pencil marks?

Is it the expected behaviour that resetting a fixed is an error but reseting a solved is ignored
 and should edit mode stop or reset the solver?

Undo

Load state, show errors in loaded state or puzzle

Some things don't make sense - can't show contradictions informatively when there are no pencil marks

Set hint as a result of changing the board or pressing 'hint' and move selection to it
 or clear selection if hint is that a solution is found or impossible

Hint options
 Entering
  Modes - edit, enter value, toggle pencil mark
  Show hint automatically
   Solved
   Impossible
   One way to fulfil requirement
   Apply the above hints automatically
   Advanced hints
 Solving and puzzle generation
  Which advanced hints to use, in which order, difficulty rating
   Immediate contradiction
   Eventual contradiction
   Naked and hidden subsets

Rating and proof
 Based on methods required to solve by simplest method
 Proof should show only essential steps, especially when eliminating contradictions
  Proof as a guided search gives a sense of difficulty

HintSpec
 Drawing
  Manual pencil marks
  Automatic pencil marks
 Solving
 Verbal hints

Remove flag 'included'

Set focus more easily when keys pressed

Print proof
 Only show relevant steps

Look at this program
http://www.setbb.com/phpbb/viewtopic.php?mforum=sudoku&p=11241

Advice
http://www.setbb.com/phpbb/viewtopic.php?mforum=sudoku&p=11683

Try subset code
http://www.setbb.com/sudoku/viewtopic.php?t=231
and
http://www.setbb.com/phpbb/viewtopic.php?mforum=sudoku&p=10324

-

Lower priority:

Test 'solve logically'

Regression tests

Remove hint form or make it listen from the start

Move Sudoku behavious from Requirement to Sudoku requirement
 (not if it just makes things more complex)

Check for linear reasoning other than contradictions.
 If selecting a row leads to one option, give that solution, then discard the row to check for others.

When nothing else works, use backtracking.

Hints on cell using Solver.ForcedHintsOn

-

Naked Single
One way to fill a cell

Hidden Single
One way to place a number in a box, row, column

Naked and Hidden Subset, Block/Block Interactions
When there are N requirements, each with only N candidates, then candidates which conflict with all of them can be discarded
because they directly lead to no way to fulfill one of the N the requirements.

Hidden Subset
When a set of N cells in a group contains N candidates that do not appear elsewhere in the group, then other candidates in those
cells can be discarded.

Other ideas for solving
 Iterative deepening search for contradictions
