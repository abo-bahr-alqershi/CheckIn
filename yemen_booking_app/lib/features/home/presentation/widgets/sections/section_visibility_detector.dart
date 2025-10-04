// lib/features/home/presentation/widgets/analytics/section_visibility_detector.dart

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:visibility_detector/visibility_detector.dart';
import '../../bloc/home_bloc.dart';
import '../../bloc/home_event.dart';

class SectionVisibilityDetector extends StatefulWidget {
  final String sectionId;
  final Widget child;
  final Function(bool)? onVisibilityChanged;
  final double visibilityThreshold;

  const SectionVisibilityDetector({
    super.key,
    required this.sectionId,
    required this.child,
    this.onVisibilityChanged,
    this.visibilityThreshold = 0.5,
  });

  @override
  State<SectionVisibilityDetector> createState() => _SectionVisibilityDetectorState();
}

class _SectionVisibilityDetectorState extends State<SectionVisibilityDetector> {
  bool _hasRecordedImpression = false;
  bool _isVisible = false;
  DateTime? _visibilityStartTime;

  @override
  Widget build(BuildContext context) {
    return VisibilityDetector(
      key: Key('section_${widget.sectionId}'),
      onVisibilityChanged: (info) {
        final visiblePercentage = info.visibleFraction;
        final isNowVisible = visiblePercentage >= widget.visibilityThreshold;
        
        if (isNowVisible != _isVisible) {
          setState(() {
            _isVisible = isNowVisible;
          });
          
          if (isNowVisible) {
            _onSectionBecameVisible();
          } else {
            _onSectionBecameHidden();
          }
          
          widget.onVisibilityChanged?.call(isNowVisible);
        }
      },
      child: widget.child,
    );
  }

  void _onSectionBecameVisible() {
    _visibilityStartTime = DateTime.now();
    
    // Record impression if not already recorded
    if (!_hasRecordedImpression) {
      _hasRecordedImpression = true;
      context.read<HomeBloc>().add(
        RecordSectionImpressionEvent(sectionId: widget.sectionId),
      );
    }
  }

  void _onSectionBecameHidden() {
    if (_visibilityStartTime != null) {
      final duration = DateTime.now().difference(_visibilityStartTime!);
      
      // Record interaction if user spent more than 2 seconds viewing
      if (duration.inSeconds >= 2) {
        context.read<HomeBloc>().add(
          RecordSectionInteractionEvent(
            sectionId: widget.sectionId,
            interactionType: 'view',
            metadata: {
              'duration_seconds': duration.inSeconds,
            },
          ),
        );
      }
    }
    
    _visibilityStartTime = null;
  }
}