# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: mlagents/envs/communicator_objects/brain_parameters.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
from google.protobuf import descriptor_pb2
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='mlagents/envs/communicator_objects/brain_parameters.proto',
  package='communicator_objects',
  syntax='proto3',
  serialized_options=b'\252\002\034MLAgents.CommunicatorObjects',
  serialized_pb=b'\n9mlagents/envs/communicator_objects/brain_parameters.proto\x12\x14\x63ommunicator_objects\"\xf5\x01\n\x14\x42rainParametersProto\x12\x12\n\nbrain_name\x18\x01 \x01(\t\x12\x14\n\x0c\x61gents_count\x18\x02 \x01(\x05\x12 \n\x18observations_vector_size\x18\x03 \x01(\x05\x12\x1b\n\x13\x61\x63tions_vector_size\x18\x04 \x01(\x05\x12\x1f\n\x17mmf_offset_observations\x18\x05 \x01(\x05\x12\x1d\n\x15mmf_size_observations\x18\x06 \x01(\x05\x12\x1a\n\x12mmf_offset_actions\x18\x07 \x01(\x05\x12\x18\n\x10mmf_size_actions\x18\x08 \x01(\x05\x42\x1f\xaa\x02\x1cMLAgents.CommunicatorObjectsb\x06proto3'
)




_BRAINPARAMETERSPROTO = _descriptor.Descriptor(
  name='BrainParametersProto',
  full_name='communicator_objects.BrainParametersProto',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='brain_name', full_name='communicator_objects.BrainParametersProto.brain_name', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='agents_count', full_name='communicator_objects.BrainParametersProto.agents_count', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='observations_vector_size', full_name='communicator_objects.BrainParametersProto.observations_vector_size', index=2,
      number=3, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='actions_vector_size', full_name='communicator_objects.BrainParametersProto.actions_vector_size', index=3,
      number=4, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='mmf_offset_observations', full_name='communicator_objects.BrainParametersProto.mmf_offset_observations', index=4,
      number=5, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='mmf_size_observations', full_name='communicator_objects.BrainParametersProto.mmf_size_observations', index=5,
      number=6, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='mmf_offset_actions', full_name='communicator_objects.BrainParametersProto.mmf_offset_actions', index=6,
      number=7, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='mmf_size_actions', full_name='communicator_objects.BrainParametersProto.mmf_size_actions', index=7,
      number=8, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=84,
  serialized_end=329,
)

DESCRIPTOR.message_types_by_name['BrainParametersProto'] = _BRAINPARAMETERSPROTO
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

BrainParametersProto = _reflection.GeneratedProtocolMessageType('BrainParametersProto', (_message.Message,), {
  'DESCRIPTOR' : _BRAINPARAMETERSPROTO,
  '__module__' : 'mlagents.envs.communicator_objects.brain_parameters_pb2'
  # @@protoc_insertion_point(class_scope:communicator_objects.BrainParametersProto)
  })
_sym_db.RegisterMessage(BrainParametersProto)


DESCRIPTOR.has_options = True
DESCRIPTOR._options = _descriptor._ParseOptions(descriptor_pb2.FileOptions(), _b('\252\002\034MLAgents.CommunicatorObjects'))
# @@protoc_insertion_point(module_scope)
