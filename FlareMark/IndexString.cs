namespace FlareMark;

public readonly ref struct ReadOnlyIndexString(ReadOnlySpan<char> source, int index_) {
    public readonly ReadOnlySpan<char> source = source;
    public int index => index_;

    public char this[int i] {
        get {
            var idx = i + index;
            if (idx >= source.Length || idx < 0 || index == -1) {
                return '\0';
            }

            return source[idx];
        }
    }

    public ReadOnlySpan<char> this[Range range] {
        get {
            var (start, length) = range.GetOffsetAndLength(source.Length);
            start += index;
            
            if (start < 0 || length < 0 || index == -1) {
                return ReadOnlySpan<char>.Empty;
            }
            
            if (start + length > source.Length) {
                length = source.Length - start;
            }


            return source.Slice(start, length);
        }
    }

    public bool Match(IEnumerable<char> str) {
        var i = 0;
        foreach (var chr in str) {
            if (this[i] != chr) {
                return false;
            }

            i++;
        }

        return true;
    }

    public bool Match(ReadOnlySpan<char> str) {
        var i = 0;
        foreach (var chr in str) {
            if (this[i] != chr) {
                return false;
            }

            i++;
        }

        return true;
    }
    
    public bool Match(string str) {
        var i = 0;
        foreach (var chr in str) {
            if (this[i] != chr) {
                return false;
            }

            i++;
        }

        return true;
    }
}

public ref struct IndexString(ReadOnlySpan<char> source) {
    public readonly ReadOnlySpan<char> source = source;
    public int index { get; private set; } = -1;

    public char this[int i] {
        get {
            var idx = i + index;
            if (idx >= source.Length || idx < 0 || index == -1) {
                return '\0';
            }

            return source[idx];
        }
    }

    public ReadOnlySpan<char> this[Range range] {
        get {
            var (start, length) = range.GetOffsetAndLength(source.Length);
            start += index;
            
            if (start < 0 || length < 0 || index == -1) {
                return ReadOnlySpan<char>.Empty;
            }
            
            if (start + length > source.Length) {
                length = source.Length - start;
            }


            return source.Slice(start, length);
        }
    }

    public bool Move(int count) {
        if (index + count >= source.Length || index + count < 0) {
            return false;
        }

        index += count;
        return true;
    }
    
    public bool MoveNext() {
        index++;
        return index < source.Length;
    }
    
    public bool MovePrevious() {
        if (index >= source.Length || index < 0) {
            return false;
        }

        index--;
        return true;
    }

    public void Reset() {
        index = -1;
    }

    public void Dispose() { }

    public bool Match(IEnumerable<char> str) {
        var i = 0;
        foreach (var chr in str) {
            if (this[i] != chr) {
                return false;
            }

            i++;
        }

        return true;
    }

    public bool Match(ReadOnlySpan<char> str) {
        var i = 0;
        foreach (var chr in str) {
            if (this[i] != chr) {
                return false;
            }

            i++;
        }

        return true;
    }
    
    public bool Match(string str) {
        var i = 0;
        foreach (var chr in str) {
            if (this[i] != chr) {
                return false;
            }

            i++;
        }

        return true;
    }
    
    public static implicit operator ReadOnlyIndexString(IndexString str) => new(str.source, str.index);
}
