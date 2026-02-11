# Assessment Process

## How It Works

When you push code, GitHub Actions:
1. Builds your solution
2. Clones private test repository
3. Runs hidden tests against your API
4. Reports results

## What You See

- Total test count and pass/fail
- Test names and status
- Performance timings
- Build errors (if any)

## What You Don't See

- Test implementation code
- Test data details
- Expected values

## Test Categories

- **Correctness** - Finding all valid solutions
- **Performance** - Meeting time constraints
- **Tolerance** - Fuzzy matching logic
- **Validation** - Edge cases and error handling
- **Integration** - Full workflow

## Results

Results appear in the Actions tab after each push. Tests are architecture-agnostic and validate via API endpoints only.
